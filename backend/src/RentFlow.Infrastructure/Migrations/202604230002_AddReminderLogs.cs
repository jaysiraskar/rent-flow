using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentFlow.Infrastructure.Migrations;

public partial class AddReminderLogs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReminderLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                RentRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Channel = table.Column<int>(type: "int", nullable: false),
                ReminderType = table.Column<int>(type: "int", nullable: false),
                Recipient = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                Success = table.Column<bool>(type: "bit", nullable: false),
                FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReminderLogs", x => x.Id);
                table.ForeignKey("FK_ReminderLogs_RentRecords_RentRecordId", x => x.RentRecordId, "RentRecords", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_ReminderLogs_Tenants_TenantId", x => x.TenantId, "Tenants", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReminderLogs_RentRecordId_SentAtUtc",
            table: "ReminderLogs",
            columns: new[] { "RentRecordId", "SentAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_ReminderLogs_TenantId",
            table: "ReminderLogs",
            column: "TenantId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ReminderLogs");
    }
}
