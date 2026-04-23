using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Api.Extensions;
using RentFlow.Application.DTOs.Tenants;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1")]
public class TenantsController(ITenantService tenantService) : ControllerBase
{
    [HttpGet("properties/{propertyId:guid}/tenants")]
    public async Task<ActionResult<IReadOnlyCollection<TenantResponse>>> GetByProperty(Guid propertyId, CancellationToken cancellationToken)
        => Ok(await tenantService.GetByPropertyAsync(HttpContext.GetUserId(), propertyId, cancellationToken));

    [HttpPost("properties/{propertyId:guid}/tenants")]
    public async Task<ActionResult<TenantResponse>> Create(Guid propertyId, [FromBody] TenantCreateRequest request, CancellationToken cancellationToken)
    {
        TenantResponse? tenant = await tenantService.CreateAsync(HttpContext.GetUserId(), propertyId, request, cancellationToken);
        return tenant is null ? NotFound() : CreatedAtAction(nameof(GetById), new { tenantId = tenant.Id }, tenant);
    }

    [HttpGet("tenants/{tenantId:guid}")]
    public async Task<ActionResult<TenantResponse>> GetById(Guid tenantId, CancellationToken cancellationToken)
    {
        TenantResponse? tenant = await tenantService.GetByIdAsync(HttpContext.GetUserId(), tenantId, cancellationToken);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpPut("tenants/{tenantId:guid}")]
    public async Task<ActionResult<TenantResponse>> Update(Guid tenantId, [FromBody] TenantUpdateRequest request, CancellationToken cancellationToken)
    {
        TenantResponse? tenant = await tenantService.UpdateAsync(HttpContext.GetUserId(), tenantId, request, cancellationToken);
        return tenant is null ? NotFound() : Ok(tenant);
    }

    [HttpDelete("tenants/{tenantId:guid}")]
    public async Task<IActionResult> Delete(Guid tenantId, CancellationToken cancellationToken)
        => await tenantService.DeleteAsync(HttpContext.GetUserId(), tenantId, cancellationToken) ? NoContent() : NotFound();
}
