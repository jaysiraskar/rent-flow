using RentFlow.Domain.Common;

namespace RentFlow.Domain.Entities;

public class Tenant : BaseEntity
{
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string RoomOrBed { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public byte RentDueDay { get; set; }
    public DateOnly MoveInDate { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<RentRecord> RentRecords { get; set; } = new List<RentRecord>();
}
