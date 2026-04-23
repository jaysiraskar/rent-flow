using System.ComponentModel.DataAnnotations;

namespace RentFlow.Application.DTOs.Tenants;

public record TenantCreateRequest(
    [Required, MaxLength(120)] string FullName,
    [Required, MaxLength(20)] string Phone,
    [EmailAddress, MaxLength(160)] string? Email,
    [Required, MaxLength(50)] string RoomOrBed,
    [Range(1, 1000000)] decimal MonthlyRent,
    [Range(1, 28)] byte RentDueDay,
    DateOnly MoveInDate);

public record TenantUpdateRequest(
    [Required, MaxLength(120)] string FullName,
    [Required, MaxLength(20)] string Phone,
    [EmailAddress, MaxLength(160)] string? Email,
    [Required, MaxLength(50)] string RoomOrBed,
    [Range(1, 1000000)] decimal MonthlyRent,
    [Range(1, 28)] byte RentDueDay,
    bool IsActive);

public record TenantResponse(Guid Id, Guid PropertyId, string FullName, string Phone, string? Email, string RoomOrBed, decimal MonthlyRent, byte RentDueDay, bool IsActive);
