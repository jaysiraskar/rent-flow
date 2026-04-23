using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Domain.Entities;

public class RentRecord : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }
    public short BillingYear { get; set; }
    public byte BillingMonth { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal ExpectedAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public RentPaymentStatus Status { get; set; } = RentPaymentStatus.Unpaid;
    public DateTime? PaidOnUtc { get; set; }
    public string? Notes { get; set; }
}
