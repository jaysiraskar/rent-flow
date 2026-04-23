namespace RentFlow.Application.DTOs.Reminders;


public record ReminderDispatchResult(int CheckedRecords, int SentCount, int FailedCount, DateTime ProcessedAtUtc);

public record ReminderLogResponse(
    Guid Id,
    Guid RentRecordId,
    Guid TenantId,
    Guid PropertyId,
    string PropertyName,
    string TenantName,
    string Channel,
    string ReminderType,
    string Recipient,
    bool Success,
    string? FailureReason,
    DateTime SentAtUtc);
