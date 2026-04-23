using RentFlow.Domain.Common;

namespace RentFlow.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
