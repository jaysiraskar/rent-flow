using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Api.Extensions;
using RentFlow.Application.DTOs.RentRecords;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/rent-records")]
public class RentRecordsController(IRentRecordService rentRecordService) : ControllerBase
{
    [HttpPost("generate-monthly")]
    public async Task<ActionResult<object>> GenerateMonthly([FromQuery] short year, [FromQuery] byte month, [FromQuery] Guid? propertyId, CancellationToken cancellationToken)
    {
        int generated = await rentRecordService.GenerateMonthlyAsync(HttpContext.GetUserId(), year, month, propertyId, cancellationToken);
        return Ok(new { generated });
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RentRecordResponse>>> Get([FromQuery] short year, [FromQuery] byte month, [FromQuery] Guid? propertyId, [FromQuery] string? status, CancellationToken cancellationToken)
        => Ok(await rentRecordService.GetAsync(HttpContext.GetUserId(), year, month, propertyId, status, cancellationToken));

    [HttpPut("{rentRecordId:guid}/payment")]
    public async Task<ActionResult<RentRecordResponse>> UpdatePayment(Guid rentRecordId, [FromBody] RentPaymentUpdateRequest request, CancellationToken cancellationToken)
    {
        RentRecordResponse? record = await rentRecordService.UpdatePaymentAsync(HttpContext.GetUserId(), rentRecordId, request, cancellationToken);
        return record is null ? NotFound() : Ok(record);
    }
}
