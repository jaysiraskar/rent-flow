using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, IPasswordHasher hasher)
    {
        await dbContext.Database.MigrateAsync();

        if (await dbContext.Users.AnyAsync()) return;

        User landlord = new()
        {
            FullName = "Demo Landlord",
            Email = "demo@rentflow.in",
            PasswordHash = hasher.Hash("Demo@123")
        };

        Property property = new()
        {
            Landlord = landlord,
            Name = "Sunrise PG",
            AddressLine1 = "12 Residency Road",
            City = "Bengaluru",
            State = "Karnataka",
            Pincode = "560025"
        };

        dbContext.Users.Add(landlord);
        dbContext.Properties.Add(property);
        dbContext.Tenants.AddRange(
            new Tenant
            {
                Property = property,
                FullName = "Arun Kumar",
                Phone = "+919999900001",
                RoomOrBed = "Room A - Bed 1",
                MonthlyRent = 8500,
                RentDueDay = 5,
                MoveInDate = new DateOnly(2025, 10, 1)
            },
            new Tenant
            {
                Property = property,
                FullName = "Neha Singh",
                Phone = "+919999900002",
                RoomOrBed = "Room B - Bed 2",
                MonthlyRent = 9000,
                RentDueDay = 5,
                MoveInDate = new DateOnly(2025, 12, 15)
            });

        await dbContext.SaveChangesAsync();
    }
}
