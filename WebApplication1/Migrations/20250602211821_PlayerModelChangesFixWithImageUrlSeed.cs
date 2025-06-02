using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class PlayerModelChangesFixWithImageUrlSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_AspNetUsers_ApplicationUserId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_ApplicationUserId",
                table: "Players");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "ADMIN_ROLE_GUID", "ADMIN_USER_GUID" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "HERO_USER_GUID");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ADMIN_ROLE_GUID");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ADMIN_USER_GUID");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "ExperiencePoints",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "VirtualCurrencyBalance",
                table: "Players",
                newName: "PricePerHour");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Players",
                newName: "Reviews");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Players",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Players",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "84272d0d-9612-4ec0-9f25-332e001b8d47", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6f5f488d-6e4f-4a82-87ac-57056ad69dd5", 0, "354b610a-ffee-488f-af76-bf97dabc9281", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEHpciOIyuetlGMr4xj5fmf1DEwfphkTBf7W3Fuwrg6/YGtKZ+J85Bsu2tlZrUrkZdQ==", null, false, "f2c77f4d-1bdf-4feb-b07d-49141adcee45", false, "admin" });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "GamerTag", "ImageUrl", "PricePerHour", "Rating", "Reviews" },
                values: new object[] { "Top tier player with 5 years of competitive experience.", "ProGamerX", "https://example.com/images/progamerx.png", 50.00m, 4.7999999999999998, "Excellent communication and skill." });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "GamerTag", "ImageUrl", "PricePerHour", "Rating", "Reviews" },
                values: new object[] { "Specializes in strategy and team coordination.", "StrategistSupreme", "https://example.com/images/strategistsupreme.png", 40.00m, 4.5, "Great tactical mind, helped our team immensely." });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Description", "GamerTag", "ImageUrl", "PricePerHour", "Rating", "Reviews", "TeamId" },
                values: new object[] { 3, "Up and coming player, eager to learn and assist.", "NewTalent", "https://example.com/images/newtalent.png", 20.00m, 3.8999999999999999, "Good potential, friendly and responsive.", 1 });

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 2, 21, 18, 21, 125, DateTimeKind.Utc).AddTicks(3007));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 2, 21, 18, 21, 125, DateTimeKind.Utc).AddTicks(3008));

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "84272d0d-9612-4ec0-9f25-332e001b8d47", "6f5f488d-6e4f-4a82-87ac-57056ad69dd5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "84272d0d-9612-4ec0-9f25-332e001b8d47", "6f5f488d-6e4f-4a82-87ac-57056ad69dd5" });

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84272d0d-9612-4ec0-9f25-332e001b8d47");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6f5f488d-6e4f-4a82-87ac-57056ad69dd5");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Reviews",
                table: "Players",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "PricePerHour",
                table: "Players",
                newName: "VirtualCurrencyBalance");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Players",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ExperiencePoints",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "Players",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Players",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ADMIN_ROLE_GUID", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "ADMIN_USER_GUID", 0, "24a6bc3f-c97b-4eaf-b117-72bec3ee0908", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEPq8iJrNz2D9JUTUUJaJTL01pyL7sHcryj/e8UQPSQ3PSf5THvSX/bafsNIRgfm/aA==", null, false, "6ccff6fa-a62a-4eb0-821e-bf33f9d026e3", false, "admin" },
                    { "HERO_USER_GUID", 0, "8515d254-bd2c-431e-828b-36391bfd94f5", "hero@example.com", true, false, null, "HERO@EXAMPLE.COM", "HERO", "AQAAAAIAAYagAAAAEOnwIl+B1C6cp1L+uN+hRVaM6yDTgv0sRSCEIMPzxIABik17HNv3bT6SPqzYJSoGdg==", null, false, "03874ea1-852e-4e78-a222-b2cf6544bd1f", false, "hero" }
                });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApplicationUserId", "Description", "Email", "ExperiencePoints", "GamerTag", "ImageUrl", "LastLoginDate", "Level", "RegistrationDate", "VirtualCurrencyBalance" },
                values: new object[] { "ADMIN_USER_GUID", "The admin's player profile", "admin@example.com", 10000L, "AdminPlayer", null, new DateTime(2025, 6, 2, 20, 35, 21, 298, DateTimeKind.Utc).AddTicks(844), 10, new DateTime(2025, 6, 2, 20, 35, 21, 298, DateTimeKind.Utc).AddTicks(844), 500.00m });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApplicationUserId", "Description", "Email", "ExperiencePoints", "GamerTag", "ImageUrl", "LastLoginDate", "Level", "RegistrationDate", "VirtualCurrencyBalance" },
                values: new object[] { "HERO_USER_GUID", "A heroic player", "hero@example.com", 2500L, "HeroPlayer", null, null, 5, new DateTime(2025, 6, 2, 20, 35, 21, 298, DateTimeKind.Utc).AddTicks(850), 100.00m });

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 2, 20, 35, 21, 298, DateTimeKind.Utc).AddTicks(823));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 2, 20, 35, 21, 298, DateTimeKind.Utc).AddTicks(825));

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "ADMIN_ROLE_GUID", "ADMIN_USER_GUID" });

            migrationBuilder.CreateIndex(
                name: "IX_Players_ApplicationUserId",
                table: "Players",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_AspNetUsers_ApplicationUserId",
                table: "Players",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
