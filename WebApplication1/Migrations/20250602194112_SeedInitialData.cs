using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "730207cf-6cc7-460d-bdce-e70279fd9730", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e2f80499-7b5c-4018-9543-cbdae02d06d6", 0, "76277bb5-9716-43ff-ae5b-6f37ae81ba87", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEMCPViSJ1SaKgBghtmVG547LQ+MwfamdbzhTx1CYtLliFmkOyFjcZATm7m65n7E+/g==", null, false, "4feb5df3-07e6-42b4-89ca-6215e9fe8af5", false, "admin" });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "DateCreated", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 2, 19, 41, 12, 718, DateTimeKind.Utc).AddTicks(4254), "The first team", "Team Alpha" },
                    { 2, new DateTime(2025, 6, 2, 19, 41, 12, 718, DateTimeKind.Utc).AddTicks(4256), "The second team", "Team Bravo" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "730207cf-6cc7-460d-bdce-e70279fd9730", "e2f80499-7b5c-4018-9543-cbdae02d06d6" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "ApplicationUserId", "Description", "Email", "ExperiencePoints", "GamerTag", "LastLoginDate", "Level", "RegistrationDate", "TeamId", "VirtualCurrencyBalance" },
                values: new object[,]
                {
                    { 1, "e2f80499-7b5c-4018-9543-cbdae02d06d6", "The admin's player profile", "admin@example.com", 10000L, "AdminPlayer", new DateTime(2025, 6, 2, 19, 41, 12, 718, DateTimeKind.Utc).AddTicks(4273), 10, new DateTime(2025, 6, 2, 19, 41, 12, 718, DateTimeKind.Utc).AddTicks(4272), 1, 500.00m },
                    { 2, "9163444f-a784-43df-8224-7b4ba022d67c", "A heroic player", "hero@example.com", 2500L, "HeroPlayer", null, 5, new DateTime(2025, 6, 2, 19, 41, 12, 718, DateTimeKind.Utc).AddTicks(4278), 2, 100.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "730207cf-6cc7-460d-bdce-e70279fd9730", "e2f80499-7b5c-4018-9543-cbdae02d06d6" });

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "730207cf-6cc7-460d-bdce-e70279fd9730");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e2f80499-7b5c-4018-9543-cbdae02d06d6");

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
