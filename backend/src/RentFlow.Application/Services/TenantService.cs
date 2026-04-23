using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.Tenants;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Services;

public class TenantService(IAppDbContext dbContext) : ITenantService
{
    public async Task<IReadOnlyCollection<TenantResponse>> GetByPropertyAsync(Guid landlordId, Guid propertyId, CancellationToken cancellationToken = default)
        => await dbContext.Tenants
            .Where(t => t.PropertyId == propertyId && t.Property!.LandlordId == landlordId)
            .OrderBy(t => t.FullName)
            .Select(t => new TenantResponse(t.Id, t.PropertyId, t.FullName, t.Phone, t.Email, t.RoomOrBed, t.MonthlyRent, t.RentDueDay, t.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<TenantResponse?> GetByIdAsync(Guid landlordId, Guid tenantId, CancellationToken cancellationToken = default)
        => await dbContext.Tenants
            .Where(t => t.Id == tenantId && t.Property!.LandlordId == landlordId)
            .Select(t => new TenantResponse(t.Id, t.PropertyId, t.FullName, t.Phone, t.Email, t.RoomOrBed, t.MonthlyRent, t.RentDueDay, t.IsActive))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<TenantResponse?> CreateAsync(Guid landlordId, Guid propertyId, TenantCreateRequest request, CancellationToken cancellationToken = default)
    {
        var hasProperty = await dbContext.Properties.AnyAsync(p => p.Id == propertyId && p.LandlordId == landlordId, cancellationToken);
        if (!hasProperty) return null;

        var tenant = new Tenant
        {
            PropertyId = propertyId,
            FullName = request.FullName,
            Phone = request.Phone,
            Email = request.Email,
            RoomOrBed = request.RoomOrBed,
            MonthlyRent = request.MonthlyRent,
            RentDueDay = request.RentDueDay,
            MoveInDate = request.MoveInDate
        };
        dbContext.Tenants.Add(tenant);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new TenantResponse(tenant.Id, tenant.PropertyId, tenant.FullName, tenant.Phone, tenant.Email, tenant.RoomOrBed, tenant.MonthlyRent, tenant.RentDueDay, tenant.IsActive);
    }

    public async Task<TenantResponse?> UpdateAsync(Guid landlordId, Guid tenantId, TenantUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants.Include(t => t.Property)
            .FirstOrDefaultAsync(t => t.Id == tenantId && t.Property!.LandlordId == landlordId, cancellationToken);
        if (tenant is null) return null;

        tenant.FullName = request.FullName;
        tenant.Phone = request.Phone;
        tenant.Email = request.Email;
        tenant.RoomOrBed = request.RoomOrBed;
        tenant.MonthlyRent = request.MonthlyRent;
        tenant.RentDueDay = request.RentDueDay;
        tenant.IsActive = request.IsActive;
        tenant.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new TenantResponse(tenant.Id, tenant.PropertyId, tenant.FullName, tenant.Phone, tenant.Email, tenant.RoomOrBed, tenant.MonthlyRent, tenant.RentDueDay, tenant.IsActive);
    }

    public async Task<bool> DeleteAsync(Guid landlordId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants.Include(t => t.Property)
            .FirstOrDefaultAsync(t => t.Id == tenantId && t.Property!.LandlordId == landlordId, cancellationToken);
        if (tenant is null) return false;

        dbContext.Tenants.Remove(tenant);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
