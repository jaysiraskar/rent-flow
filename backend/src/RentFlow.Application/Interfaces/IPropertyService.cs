using RentFlow.Application.DTOs.Properties;

namespace RentFlow.Application.Interfaces;

public interface IPropertyService
{
    Task<IReadOnlyCollection<PropertyResponse>> GetAllAsync(Guid landlordId, CancellationToken cancellationToken = default);
    Task<PropertyResponse?> GetByIdAsync(Guid landlordId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<PropertyResponse> CreateAsync(Guid landlordId, PropertyCreateRequest request, CancellationToken cancellationToken = default);
    Task<PropertyResponse?> UpdateAsync(Guid landlordId, Guid propertyId, PropertyUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid landlordId, Guid propertyId, CancellationToken cancellationToken = default);
}
