using Microsoft.EntityFrameworkCore;
using RentFlow.Domain.Entities;

namespace RentFlow.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Property> Properties { get; }
    DbSet<Tenant> Tenants { get; }
    DbSet<RentRecord> RentRecords { get; }
    DbSet<ReminderLog> ReminderLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
