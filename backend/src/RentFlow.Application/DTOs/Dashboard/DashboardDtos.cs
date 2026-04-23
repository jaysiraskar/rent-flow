namespace RentFlow.Application.DTOs.Dashboard;

public record DashboardSummaryResponse(int TotalTenants, decimal TotalDue, decimal CollectedAmount, decimal PendingAmount);
