using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagementSystem.Identity.Migrations
{
    /// <inheritdoc />
    public partial class NewIdentityMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c1122f1a-3748-466b-af45-ac3992d2e121"),
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("c1122f1a-3748-466b-af45-ac3992d2e121"),
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Attendee", "ATTENDEE" });
        }
    }
}
