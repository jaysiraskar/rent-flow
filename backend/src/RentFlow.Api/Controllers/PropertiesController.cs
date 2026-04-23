using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Api.Extensions;
using RentFlow.Application.DTOs.Properties;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/properties")]
public class PropertiesController(IPropertyService propertyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PropertyResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await propertyService.GetAllAsync(HttpContext.GetUserId(), cancellationToken));

    [HttpGet("{propertyId:guid}")]
    public async Task<ActionResult<PropertyResponse>> GetById(Guid propertyId, CancellationToken cancellationToken)
    {
        PropertyResponse? property = await propertyService.GetByIdAsync(HttpContext.GetUserId(), propertyId, cancellationToken);
        return property is null ? NotFound() : Ok(property);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyResponse>> Create([FromBody] PropertyCreateRequest request, CancellationToken cancellationToken)
    {
        PropertyResponse property = await propertyService.CreateAsync(HttpContext.GetUserId(), request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { propertyId = property.Id }, property);
    }

    [HttpPut("{propertyId:guid}")]
    public async Task<ActionResult<PropertyResponse>> Update(Guid propertyId, [FromBody] PropertyUpdateRequest request, CancellationToken cancellationToken)
    {
        PropertyResponse? property = await propertyService.UpdateAsync(HttpContext.GetUserId(), propertyId, request, cancellationToken);
        return property is null ? NotFound() : Ok(property);
    }

    [HttpDelete("{propertyId:guid}")]
    public async Task<IActionResult> Delete(Guid propertyId, CancellationToken cancellationToken)
        => await propertyService.DeleteAsync(HttpContext.GetUserId(), propertyId, cancellationToken) ? NoContent() : NotFound();
}
