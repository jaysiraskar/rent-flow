using RentFlow.Application.DTOs.Dashboard;

namespace RentFlow.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync(Guid landlordId, short year, byte month, Guid? propertyId, CancellationToken cancellationToken = default);
}
