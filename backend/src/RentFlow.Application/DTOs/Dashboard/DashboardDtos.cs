namespace RentFlow.Application.DTOs.Dashboard;

public record DashboardSummaryResponse(int TotalTenants, decimal TotalDue, decimal CollectedAmount, decimal PendingAmount);

public record DashboardDueItemResponse(
    Guid RentRecordId,
    Guid TenantId,
    Guid PropertyId,
    string TenantName,
    string PropertyName,
    DateOnly DueDate,
    decimal ExpectedAmount,
    decimal PaidAmount,
    decimal PendingAmount,
    string Status);
