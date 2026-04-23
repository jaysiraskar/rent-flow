using RentFlow.Application.DTOs.Dashboard;

namespace RentFlow.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(Guid landlordId, short year, byte month, Guid? propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DashboardDueItemResponse>> GetUpcomingDuesAsync(Guid landlordId, int days = 7, Guid? propertyId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DashboardDueItemResponse>> GetOverdueAsync(Guid landlordId, Guid? propertyId = null, CancellationToken cancellationToken = default);
}
