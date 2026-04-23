using RentFlow.Application.DTOs.RentRecords;

namespace RentFlow.Application.Interfaces;

public interface IRentRecordService
{
    Task<int> GenerateMonthlyAsync(Guid landlordId, short year, byte month, Guid? propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RentRecordResponse>> GetAsync(Guid landlordId, short year, byte month, Guid? propertyId, string? status, CancellationToken cancellationToken = default);
    Task<RentRecordResponse?> UpdatePaymentAsync(Guid landlordId, Guid rentRecordId, RentPaymentUpdateRequest request, CancellationToken cancellationToken = default);
}
