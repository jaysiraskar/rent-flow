using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.Dashboard;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;

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

    public async Task<IReadOnlyCollection<DashboardDueItemResponse>> GetUpcomingDuesAsync(Guid landlordId, int days = 7, Guid? propertyId = null, CancellationToken cancellationToken = default)
    {
        int effectiveDays = Math.Clamp(days, 1, 60);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly threshold = today.AddDays(effectiveDays);

        IQueryable<RentRecord> query = dbContext.RentRecords
            .Where(r => r.Property!.LandlordId == landlordId)
            .Where(r => r.Status != RentPaymentStatus.Paid)
            .Where(r => r.DueDate >= today && r.DueDate <= threshold);

        if (propertyId.HasValue)
            query = query.Where(r => r.PropertyId == propertyId.Value);

        return await query
            .OrderBy(r => r.DueDate)
            .Select(r => new DashboardDueItemResponse(
                r.Id,
                r.TenantId,
                r.PropertyId,
                r.Tenant!.FullName,
                r.Property!.Name,
                r.DueDate,
                r.ExpectedAmount,
                r.PaidAmount,
                r.ExpectedAmount - r.PaidAmount,
                r.Status.ToString()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardDueItemResponse>> GetOverdueAsync(Guid landlordId, Guid? propertyId = null, CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

        IQueryable<RentRecord> query = dbContext.RentRecords
            .Where(r => r.Property!.LandlordId == landlordId)
            .Where(r => r.Status != RentPaymentStatus.Paid)
            .Where(r => r.DueDate < today);

        if (propertyId.HasValue)
            query = query.Where(r => r.PropertyId == propertyId.Value);

        return await query
            .OrderBy(r => r.DueDate)
            .Select(r => new DashboardDueItemResponse(
                r.Id,
                r.TenantId,
                r.PropertyId,
                r.Tenant!.FullName,
                r.Property!.Name,
                r.DueDate,
                r.ExpectedAmount,
                r.PaidAmount,
                r.ExpectedAmount - r.PaidAmount,
                r.Status.ToString()))
            .ToListAsync(cancellationToken);
    }
}
