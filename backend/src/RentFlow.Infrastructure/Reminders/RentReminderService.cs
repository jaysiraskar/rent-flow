using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentFlow.Application.DTOs.Reminders;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;
using RentFlow.Infrastructure.Data;

namespace RentFlow.Infrastructure.Reminders;

public class RentReminderService(
    AppDbContext dbContext,
    IEnumerable<INotificationChannel> channels,
    IOptions<ReminderOptions> reminderOptions,
    ILogger<RentReminderService> logger) : IRentReminderService
{
    public async Task<ReminderDispatchResult> ProcessDueRemindersAsync(CancellationToken cancellationToken = default)
    {
        INotificationChannel? emailChannel = channels.FirstOrDefault(c => c.ChannelName.Equals("Email", StringComparison.OrdinalIgnoreCase));
        if (emailChannel is null)
        {
            logger.LogWarning("No email notification channel registered.");
            return new ReminderDispatchResult(0, 0, 0, DateTime.UtcNow);
        }

        DateTime now = DateTime.UtcNow;
        DateOnly today = DateOnly.FromDateTime(now);
        DateOnly upcomingThreshold = today.AddDays(reminderOptions.Value.UpcomingDaysThreshold);

        List<RentRecord> candidates = await dbContext.RentRecords
            .Include(x => x.Tenant)
            .Where(x => x.Status != RentPaymentStatus.Paid && x.Tenant!.IsActive)
            .Where(x => x.DueDate <= upcomingThreshold)
            .ToListAsync(cancellationToken);

        int sent = 0;
        int failed = 0;

        foreach (RentRecord? record in candidates)
        {
            ReminderType reminderType = record.DueDate < today ? ReminderType.Overdue : ReminderType.Upcoming;
            bool alreadySentToday = await dbContext.ReminderLogs.AnyAsync(
                x => x.RentRecordId == record.Id
                    && x.Channel == ReminderChannel.Email
                    && x.ReminderType == reminderType
                    && x.SentAtUtc.Date == now.Date,
                cancellationToken);

            if (alreadySentToday) continue;

            string? recipient = record.Tenant?.Email;
            if (string.IsNullOrWhiteSpace(recipient))
            {
                await LogAsync(record, reminderType, ReminderChannel.Email, "N/A", "Skipped: no email", false, "Tenant email missing", cancellationToken);
                failed++;
                continue;
            }

            string subject = reminderType == ReminderType.Upcoming
                ? "RentFlow: Upcoming Rent Due"
                : "RentFlow: Rent Payment Overdue";

            string message = reminderType == ReminderType.Upcoming
                ? $"Hi {record.Tenant!.FullName}, your rent of INR {record.ExpectedAmount} is due on {record.DueDate}."
                : $"Hi {record.Tenant!.FullName}, your rent of INR {record.ExpectedAmount} was due on {record.DueDate} and is still pending.";

            try
            {
                await emailChannel.SendAsync(recipient, subject, message, cancellationToken);
                await LogAsync(record, reminderType, ReminderChannel.Email, recipient, message, true, null, cancellationToken);
                sent++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send reminder for rentRecord {RentRecordId}", record.Id);
                await LogAsync(record, reminderType, ReminderChannel.Email, recipient, message, false, ex.Message, cancellationToken);
                failed++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new ReminderDispatchResult(candidates.Count, sent, failed, DateTime.UtcNow);
    }

    private async Task LogAsync(RentRecord record, ReminderType type, ReminderChannel channel, string recipient, string message, bool success, string? reason, CancellationToken cancellationToken)
    {
        await dbContext.ReminderLogs.AddAsync(new ReminderLog
        {
            RentRecordId = record.Id,
            TenantId = record.TenantId,
            ReminderType = type,
            Channel = channel,
            Recipient = recipient,
            Message = message,
            Success = success,
            FailureReason = reason,
            SentAtUtc = DateTime.UtcNow
        }, cancellationToken);
    }
}
