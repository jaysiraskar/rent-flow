using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Api.Extensions;
using RentFlow.Application.DTOs.Dashboard;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/dashboard")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetSummary([FromQuery] short year, [FromQuery] byte month, [FromQuery] Guid? propertyId, CancellationToken cancellationToken)
        => Ok(await dashboardService.GetSummaryAsync(HttpContext.GetUserId(), year, month, propertyId, cancellationToken));

    [HttpGet("upcoming-dues")]
    public async Task<ActionResult<IReadOnlyCollection<DashboardDueItemResponse>>> GetUpcomingDues([FromQuery] int days = 7, [FromQuery] Guid? propertyId = null, CancellationToken cancellationToken = default)
        => Ok(await dashboardService.GetUpcomingDuesAsync(HttpContext.GetUserId(), days, propertyId, cancellationToken));

    [HttpGet("overdue")]
    public async Task<ActionResult<IReadOnlyCollection<DashboardDueItemResponse>>> GetOverdue([FromQuery] Guid? propertyId = null, CancellationToken cancellationToken = default)
        => Ok(await dashboardService.GetOverdueAsync(HttpContext.GetUserId(), propertyId, cancellationToken));
}
