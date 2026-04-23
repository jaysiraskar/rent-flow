using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Auth;

public class JwtTokenGenerator(IConfiguration configuration) : IAuthTokenGenerator
{
    public string Generate(User user)
    {
        var key = configuration["Jwt:Key"] ?? "dev-only-super-secret-key-change-me";
        var issuer = configuration["Jwt:Issuer"] ?? "RentFlow";
        var audience = configuration["Jwt:Audience"] ?? "RentFlow.Client";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("name", user.FullName)
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
