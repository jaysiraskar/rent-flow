using System.ComponentModel.DataAnnotations;

namespace RentFlow.Application.DTOs.Auth;

public record RegisterRequest(
    [Required, MaxLength(120)] string FullName,
    [Required, EmailAddress, MaxLength(160)] string Email,
    [Required, MinLength(8)] string Password,
    [MaxLength(20)] string? PhoneNumber);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record AuthResponse(string Token, string FullName, string Email);
