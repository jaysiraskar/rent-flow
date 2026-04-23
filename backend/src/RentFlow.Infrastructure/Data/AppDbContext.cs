using Microsoft.EntityFrameworkCore;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Entities;

namespace RentFlow.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<RentRecord> RentRecords => Set<RentRecord>();
    public DbSet<ReminderLog> ReminderLogs => Set<ReminderLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(160).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(120).IsRequired();
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasOne(x => x.Landlord).WithMany(x => x.Properties).HasForeignKey(x => x.LandlordId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.AddressLine1).HasMaxLength(200).IsRequired();
            entity.Property(x => x.City).HasMaxLength(100).IsRequired();
            entity.Property(x => x.State).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Pincode).HasMaxLength(10).IsRequired();
            entity.HasIndex(x => x.LandlordId);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasOne(x => x.Property).WithMany(x => x.Tenants).HasForeignKey(x => x.PropertyId).OnDelete(DeleteBehavior.Cascade);
            entity.Property(x => x.FullName).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(20).IsRequired();
            entity.Property(x => x.RoomOrBed).HasMaxLength(50).IsRequired();
            entity.Property(x => x.MonthlyRent).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.PropertyId, x.IsActive });
        });

        modelBuilder.Entity<RentRecord>(entity =>
        {
            entity.HasOne(x => x.Tenant).WithMany(x => x.RentRecords).HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Property).WithMany().HasForeignKey(x => x.PropertyId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(x => x.ExpectedAmount).HasPrecision(18, 2);
            entity.Property(x => x.PaidAmount).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.PropertyId, x.BillingYear, x.BillingMonth, x.Status });
            entity.HasIndex(x => new { x.TenantId, x.BillingYear, x.BillingMonth }).IsUnique();
        });

        modelBuilder.Entity<ReminderLog>(entity =>
        {
            entity.HasOne(x => x.RentRecord).WithMany().HasForeignKey(x => x.RentRecordId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(x => x.Recipient).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.FailureReason).HasMaxLength(500);
            entity.HasIndex(x => new { x.RentRecordId, x.SentAtUtc });
        });
    }
}
