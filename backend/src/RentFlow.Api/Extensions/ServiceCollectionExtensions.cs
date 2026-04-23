using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentFlow.Api.BackgroundJobs;
using RentFlow.Application.Interfaces;
using RentFlow.Application.Services;
using RentFlow.Infrastructure.Auth;
using RentFlow.Infrastructure.Data;
using RentFlow.Infrastructure.Reminders;

namespace RentFlow.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IRentRecordService, RentRecordService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IRentReminderService, RentReminderService>();
        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<INotificationChannel, EmailNotificationChannel>();

        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        services.Configure<ReminderOptions>(configuration.GetSection("Reminders"));
        services.AddHostedService<RentReminderHostedService>();

        var key = configuration["Jwt:Key"] ?? "dev-only-super-secret-key-change-me";
        if (key.Length < 32)
            throw new InvalidOperationException("JWT key must be at least 32 characters.");

        var issuer = configuration["Jwt:Issuer"] ?? "RentFlow";
        var audience = configuration["Jwt:Audience"] ?? "RentFlow.Client";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    NameClaimType = "sub"
                };
            });

        return services;
    }
}
