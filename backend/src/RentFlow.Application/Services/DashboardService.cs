using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.Dashboard;
using RentFlow.Application.Interfaces;

namespace RentFlow.Application.Services;

public class DashboardService(IAppDbContext dbContext) : IDashboardService
{
    public async Task<DashboardSummaryResponse> GetSummaryAsync(Guid landlordId, short year, byte month, Guid? propertyId, CancellationToken cancellationToken = default)
    {
        var tenantQuery = dbContext.Tenants.Where(t => t.IsActive && t.Property!.LandlordId == landlordId);
        if (propertyId.HasValue)
            tenantQuery = tenantQuery.Where(t => t.PropertyId == propertyId.Value);

        var rentQuery = dbContext.RentRecords.Where(r => r.BillingYear == year && r.BillingMonth == month && r.Property!.LandlordId == landlordId);
        if (propertyId.HasValue)
            rentQuery = rentQuery.Where(r => r.PropertyId == propertyId.Value);

        var totalTenants = await tenantQuery.CountAsync(cancellationToken);
        var totalDue = await rentQuery.SumAsync(r => (decimal?)r.ExpectedAmount, cancellationToken) ?? 0;
        var collected = await rentQuery.SumAsync(r => (decimal?)r.PaidAmount, cancellationToken) ?? 0;

        return new DashboardSummaryResponse(totalTenants, totalDue, collected, totalDue - collected);
    }
}
