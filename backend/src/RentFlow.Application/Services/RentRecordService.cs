using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.RentRecords;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;
using RentFlow.Domain.Enums;

namespace RentFlow.Application.Services;

public class RentRecordService(IAppDbContext dbContext) : IRentRecordService
{
    public async Task<int> GenerateMonthlyAsync(Guid landlordId, short year, byte month, Guid? propertyId, CancellationToken cancellationToken = default)
    {
        if (month is < 1 or > 12) throw new InvalidOperationException("Month must be between 1 and 12.");
        if (year is < 2000 or > 2100) throw new InvalidOperationException("Year is out of allowed range.");

        var tenantsQuery = dbContext.Tenants
            .Where(t => t.IsActive && t.Property!.LandlordId == landlordId);

        if (propertyId.HasValue)
            tenantsQuery = tenantsQuery.Where(t => t.PropertyId == propertyId.Value);

        var tenants = await tenantsQuery.ToListAsync(cancellationToken);
        var tenantIds = tenants.Select(t => t.Id).ToList();
        var existingIds = await dbContext.RentRecords
            .Where(r => r.BillingYear == year && r.BillingMonth == month && tenantIds.Contains(r.TenantId))
            .Select(r => r.TenantId)
            .ToHashSetAsync(cancellationToken);

        var created = 0;
        foreach (var tenant in tenants.Where(t => !existingIds.Contains(t.Id)))
        {
            dbContext.RentRecords.Add(new RentRecord
            {
                TenantId = tenant.Id,
                PropertyId = tenant.PropertyId,
                BillingYear = year,
                BillingMonth = month,
                DueDate = new DateOnly(year, month, tenant.RentDueDay),
                ExpectedAmount = tenant.MonthlyRent,
                PaidAmount = 0,
                Status = RentPaymentStatus.Unpaid
            });
            created++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return created;
    }

    public async Task<IReadOnlyCollection<RentRecordResponse>> GetAsync(Guid landlordId, short year, byte month, Guid? propertyId, string? status, CancellationToken cancellationToken = default)
    {
        var query = dbContext.RentRecords
            .Where(r => r.BillingYear == year && r.BillingMonth == month && r.Property!.LandlordId == landlordId)
            .Include(r => r.Tenant)
            .Include(r => r.Property)
            .AsQueryable();

        if (propertyId.HasValue) query = query.Where(r => r.PropertyId == propertyId.Value);
        if (Enum.TryParse<RentPaymentStatus>(status ?? string.Empty, true, out var parsedStatus))
            query = query.Where(r => r.Status == parsedStatus);

        return await query
            .OrderBy(r => r.DueDate)
            .Select(r => new RentRecordResponse(
                r.Id, r.TenantId, r.PropertyId, r.BillingYear, r.BillingMonth, r.DueDate,
                r.ExpectedAmount, r.PaidAmount, r.Status.ToString(), r.PaidOnUtc, r.Notes,
                r.Tenant!.FullName, r.Property!.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<RentRecordResponse?> UpdatePaymentAsync(Guid landlordId, Guid rentRecordId, RentPaymentUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var record = await dbContext.RentRecords
            .Include(r => r.Tenant)
            .Include(r => r.Property)
            .FirstOrDefaultAsync(r => r.Id == rentRecordId && r.Property!.LandlordId == landlordId, cancellationToken);

        if (record is null) return null;

        record.PaidAmount = request.PaidAmount;
        record.PaidOnUtc = request.PaidOnUtc ?? DateTime.UtcNow;
        record.Notes = request.Notes?.Trim();
        record.Status = RentStatusCalculator.Calculate(record.ExpectedAmount, request.PaidAmount);
        record.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new RentRecordResponse(
            record.Id, record.TenantId, record.PropertyId, record.BillingYear, record.BillingMonth, record.DueDate,
            record.ExpectedAmount, record.PaidAmount, record.Status.ToString(), record.PaidOnUtc, record.Notes,
            record.Tenant!.FullName, record.Property!.Name);
    }
}
