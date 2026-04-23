using RentFlow.Domain.Common;

namespace RentFlow.Domain.Entities;

public class Property : BaseEntity
{
    public Guid LandlordId { get; set; }
    public User? Landlord { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
