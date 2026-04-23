using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentFlow.Infrastructure.Data;

#nullable disable

namespace RentFlow.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.8")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity("RentFlow.Domain.Entities.User", b =>
        {
            b.Property<Guid>("Id");
            b.Property<DateTime>("CreatedAtUtc");
            b.Property<string>("Email").HasMaxLength(160);
            b.Property<string>("FullName").HasMaxLength(120);
            b.Property<string>("PasswordHash");
            b.Property<string>("PhoneNumber");
            b.Property<DateTime>("UpdatedAtUtc");
            b.HasKey("Id");
            b.HasIndex("Email").IsUnique();
            b.ToTable("Users");
        });
    }
}
