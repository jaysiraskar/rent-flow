using Microsoft.EntityFrameworkCore;
using RentFlow.Application.DTOs.Auth;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Services;

public class AuthService(IAppDbContext dbContext, IPasswordHasher passwordHasher, IAuthTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        string email = request.Email.Trim().ToLowerInvariant();
        bool exists = await dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (exists) throw new InvalidOperationException("Email already registered.");

        User user = new()
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            PhoneNumber = request.PhoneNumber
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        string token = tokenGenerator.Generate(user);
        return new AuthResponse(token, user.FullName, user.Email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        string email = request.Email.Trim().ToLowerInvariant();
        User user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return new AuthResponse(tokenGenerator.Generate(user), user.FullName, user.Email);
    }
}
