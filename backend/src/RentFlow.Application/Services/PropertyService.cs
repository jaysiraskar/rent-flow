using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.Properties;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Services;

public class PropertyService(IAppDbContext dbContext) : IPropertyService
{
    public async Task<IReadOnlyCollection<PropertyResponse>> GetAllAsync(Guid landlordId, CancellationToken cancellationToken = default)
        => await dbContext.Properties
            .Where(x => x.LandlordId == landlordId)
            .OrderBy(x => x.Name)
            .Select(x => new PropertyResponse(x.Id, x.Name, x.AddressLine1, x.City, x.State, x.Pincode))
            .ToListAsync(cancellationToken);

    public async Task<PropertyResponse?> GetByIdAsync(Guid landlordId, Guid propertyId, CancellationToken cancellationToken = default)
        => await dbContext.Properties
            .Where(x => x.LandlordId == landlordId && x.Id == propertyId)
            .Select(x => new PropertyResponse(x.Id, x.Name, x.AddressLine1, x.City, x.State, x.Pincode))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PropertyResponse> CreateAsync(Guid landlordId, PropertyCreateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Property
        {
            LandlordId = landlordId,
            Name = request.Name,
            AddressLine1 = request.AddressLine1,
            City = request.City,
            State = request.State,
            Pincode = request.Pincode
        };
        dbContext.Properties.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new PropertyResponse(entity.Id, entity.Name, entity.AddressLine1, entity.City, entity.State, entity.Pincode);
    }

    public async Task<PropertyResponse?> UpdateAsync(Guid landlordId, Guid propertyId, PropertyUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Properties.FirstOrDefaultAsync(x => x.Id == propertyId && x.LandlordId == landlordId, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.Name;
        entity.AddressLine1 = request.AddressLine1;
        entity.City = request.City;
        entity.State = request.State;
        entity.Pincode = request.Pincode;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new PropertyResponse(entity.Id, entity.Name, entity.AddressLine1, entity.City, entity.State, entity.Pincode);
    }

    public async Task<bool> DeleteAsync(Guid landlordId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Properties.Include(x => x.Tenants)
            .FirstOrDefaultAsync(x => x.Id == propertyId && x.LandlordId == landlordId, cancellationToken);
        if (entity is null) return false;
        if (entity.Tenants.Any(t => t.IsActive)) throw new InvalidOperationException("Cannot delete property with active tenants.");
        dbContext.Properties.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
