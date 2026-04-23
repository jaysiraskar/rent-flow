using RentFlow.Application.DTOs.Tenants;

namespace RentFlow.Application.Interfaces;

public interface ITenantService
{
    Task<IReadOnlyCollection<TenantResponse>> GetByPropertyAsync(Guid landlordId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<TenantResponse?> GetByIdAsync(Guid landlordId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<TenantResponse?> CreateAsync(Guid landlordId, Guid propertyId, TenantCreateRequest request, CancellationToken cancellationToken = default);
    Task<TenantResponse?> UpdateAsync(Guid landlordId, Guid tenantId, TenantUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid landlordId, Guid tenantId, CancellationToken cancellationToken = default);
}
