using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class TeamDbMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0ad726b4-1390-4bbe-bc81-712bfbc6ff7f", "AQAAAAIAAYagAAAAEMSbE0hrn13L8gxfhTsyZyJhM7j6lUq28YT9algRPXEaZIlm/M2bqX76yYK22iGT7w==" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 30, 12, 34, 23, 260, DateTimeKind.Utc).AddTicks(6558));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 2, 12, 34, 23, 260, DateTimeKind.Utc).AddTicks(6564));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 1, 12, 34, 23, 260, DateTimeKind.Utc).AddTicks(6565));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 12, 34, 23, 260, DateTimeKind.Utc).AddTicks(6506));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 12, 34, 23, 260, DateTimeKind.Utc).AddTicks(6513));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b2492a15-9354-4b4b-a24e-a5989e94fa9b", "AQAAAAIAAYagAAAAEO1UN9ILQT0iL3r9tLYrSMhJ6cAOwu4HRnKFU9ihMgOufcZGB8gaXQPli/fYu+IIzQ==" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 30, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(819));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 2, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(825));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 1, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(827));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(769));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(775));
        }
    }
}
