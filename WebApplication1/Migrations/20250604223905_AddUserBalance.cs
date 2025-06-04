using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "Balance", "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { 0.00m, "9d0c0cbb-bd0e-4ed4-a6f6-87eb7197a124", "AQAAAAIAAYagAAAAEKZ2SRi2ZFKHnL6i4kCbP2UTlE4WCf9xEuGtx5pRzKpyljr+Db7aFR2yInYogfnLfg==" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 30, 22, 39, 4, 889, DateTimeKind.Utc).AddTicks(2037));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 2, 22, 39, 4, 889, DateTimeKind.Utc).AddTicks(2045));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 1, 22, 39, 4, 889, DateTimeKind.Utc).AddTicks(2047));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 22, 39, 4, 889, DateTimeKind.Utc).AddTicks(1983));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 22, 39, 4, 889, DateTimeKind.Utc).AddTicks(1988));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3d324c1b-ef78-46e1-951b-68098e540f84", "AQAAAAIAAYagAAAAEFUOgX7O2EkFgm0KdJoN5btHJLA+GpaVEHXTWvWRgkJotmpW78+sZNRDSvkvIQXzrA==" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 30, 21, 50, 13, 597, DateTimeKind.Utc).AddTicks(1272));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 2, 21, 50, 13, 597, DateTimeKind.Utc).AddTicks(1280));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 1, 21, 50, 13, 597, DateTimeKind.Utc).AddTicks(1281));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 21, 50, 13, 597, DateTimeKind.Utc).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 21, 50, 13, 597, DateTimeKind.Utc).AddTicks(1226));
        }
    }
}
