using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.Dashboard;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Services;

public class DashboardService(IAppDbContext dbContext) : IDashboardService
{
    public async Task<DashboardSummaryResponse> GetSummaryAsync(Guid landlordId, short year, byte month, Guid? propertyId, CancellationToken cancellationToken = default)
    {
        IQueryable<Tenant> tenantQuery = dbContext.Tenants.Where(t => t.IsActive && t.Property!.LandlordId == landlordId);
        if (propertyId.HasValue)
            tenantQuery = tenantQuery.Where(t => t.PropertyId == propertyId.Value);

        IQueryable<RentRecord> rentQuery = dbContext.RentRecords.Where(r => r.BillingYear == year && r.BillingMonth == month && r.Property!.LandlordId == landlordId);
        if (propertyId.HasValue)
            rentQuery = rentQuery.Where(r => r.PropertyId == propertyId.Value);

        int totalTenants = await tenantQuery.CountAsync(cancellationToken);
        decimal totalDue = await rentQuery.SumAsync(r => (decimal?)r.ExpectedAmount, cancellationToken) ?? 0;
        decimal collected = await rentQuery.SumAsync(r => (decimal?)r.PaidAmount, cancellationToken) ?? 0;

        return new DashboardSummaryResponse(totalTenants, totalDue, collected, totalDue - collected);
    }
}
