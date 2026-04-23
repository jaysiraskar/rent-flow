using System.ComponentModel.DataAnnotations;

namespace RentFlow.Application.DTOs.RentRecords;

public record RentRecordResponse(Guid Id, Guid TenantId, Guid PropertyId, short BillingYear, byte BillingMonth, DateOnly DueDate, decimal ExpectedAmount, decimal PaidAmount, string Status, DateTime? PaidOnUtc, string? Notes, string TenantName, string PropertyName);

public record RentPaymentUpdateRequest(
    [Range(0, 1000000)] decimal PaidAmount,
    DateTime? PaidOnUtc,
    [MaxLength(500)] string? Notes);
