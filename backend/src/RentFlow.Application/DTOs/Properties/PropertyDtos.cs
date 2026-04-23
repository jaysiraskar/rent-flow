using System.ComponentModel.DataAnnotations;

namespace RentFlow.Application.DTOs.Properties;

public record PropertyCreateRequest(
    [Required, MaxLength(120)] string Name,
    [Required, MaxLength(200)] string AddressLine1,
    [Required, MaxLength(100)] string City,
    [Required, MaxLength(100)] string State,
    [Required, MaxLength(10)] string Pincode);

public record PropertyUpdateRequest(
    [Required, MaxLength(120)] string Name,
    [Required, MaxLength(200)] string AddressLine1,
    [Required, MaxLength(100)] string City,
    [Required, MaxLength(100)] string State,
    [Required, MaxLength(10)] string Pincode);

public record PropertyResponse(Guid Id, string Name, string AddressLine1, string City, string State, string Pincode);
