using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var result = await reminderService.ProcessDueRemindersAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] int take = 100, CancellationToken cancellationToken = default)
    {
        var logs = await dbContext.ReminderLogs
            .OrderByDescending(x => x.SentAtUtc)
            .Take(Math.Clamp(take, 1, 500))
            .Select(x => new
            {
                x.Id,
                x.RentRecordId,
                x.TenantId,
                Channel = x.Channel.ToString(),
                ReminderType = x.ReminderType.ToString(),
                x.Recipient,
                x.Success,
                x.FailureReason,
                x.SentAtUtc
            })
            .ToListAsync(cancellationToken);

        return Ok(logs);
    }
}
