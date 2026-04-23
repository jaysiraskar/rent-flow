using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentFlow.Api.Extensions;
using RentFlow.Application.DTOs.Reminders;
using RentFlow.Application.Interfaces;
using RentFlow.Infrastructure.Data;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/reminders")]
public class RemindersController(IRentReminderService reminderService, AppDbContext dbContext) : ControllerBase
{
    [HttpPost("run-now")]
    public async Task<IActionResult> RunNow(CancellationToken cancellationToken)
    {
        ReminderDispatchResult result = await reminderService.ProcessDueRemindersAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<IReadOnlyCollection<ReminderLogResponse>>> GetLogs(
        [FromQuery] short? year,
        [FromQuery] byte? month,
        [FromQuery] Guid? propertyId,
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
    {
        Guid landlordId = HttpContext.GetUserId();

        IQueryable<RentFlow.Domain.Entities.ReminderLog> query = dbContext.ReminderLogs
            .AsNoTracking()
            .Where(x => x.RentRecord!.Property!.LandlordId == landlordId);

        if (year.HasValue)
        {
            query = query.Where(x => x.RentRecord!.BillingYear == year.Value);
        }

        if (month.HasValue)
        {
            query = query.Where(x => x.RentRecord!.BillingMonth == month.Value);
        }

        if (propertyId.HasValue)
        {
            query = query.Where(x => x.RentRecord!.PropertyId == propertyId.Value);
        }

        List<ReminderLogResponse> logs = await query
            .OrderByDescending(x => x.SentAtUtc)
            .Take(Math.Clamp(take, 1, 500))
            .Select(x => new ReminderLogResponse(
                x.Id,
                x.RentRecordId,
                x.TenantId,
                x.RentRecord!.PropertyId,
                x.RentRecord.Property!.Name,
                x.Tenant!.FullName,
                x.Channel.ToString(),
                x.ReminderType.ToString(),
                x.Recipient,
                x.Success,
                x.FailureReason,
                x.SentAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(logs);
    }
}
