using RentFlow.Domain.Common;
using RentFlow.Domain.Enums;

namespace RentFlow.Domain.Entities;

public class ReminderLog : BaseEntity
{
    public Guid RentRecordId { get; set; }
    public RentRecord? RentRecord { get; set; }
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    public ReminderChannel Channel { get; set; }
    public ReminderType ReminderType { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}
