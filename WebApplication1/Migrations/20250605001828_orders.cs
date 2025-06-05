using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class orders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "251488ce-b3b9-4c25-b727-356ce6393e90", "AQAAAAIAAYagAAAAEOD5MKgqcGmemqzTCFi566wghKnI4ddyO0IQGuMcqXZY306Dj8xoXTg1kWdKGYfnQQ==" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 31, 0, 18, 27, 762, DateTimeKind.Utc).AddTicks(5462));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 3, 0, 18, 27, 762, DateTimeKind.Utc).AddTicks(5469));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 2, 0, 18, 27, 762, DateTimeKind.Utc).AddTicks(5470));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 5, 0, 18, 27, 762, DateTimeKind.Utc).AddTicks(5418));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 5, 0, 18, 27, 762, DateTimeKind.Utc).AddTicks(5423));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "44d51f88-5cde-4ab0-82f2-b109ec43f3fc", "AQAAAAIAAYagAAAAEKQ2s1HpX3G5TNk4vVxEZsy/sEqIDklEZoVbFQqJsE76nWxqa0VpFstIbKMBpvpJNA==" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 30, 23, 37, 6, 244, DateTimeKind.Utc).AddTicks(6577));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 2, 23, 37, 6, 244, DateTimeKind.Utc).AddTicks(6583));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 1, 23, 37, 6, 244, DateTimeKind.Utc).AddTicks(6585));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 23, 37, 6, 244, DateTimeKind.Utc).AddTicks(6527));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 4, 23, 37, 6, 244, DateTimeKind.Utc).AddTicks(6534));
        }
    }
}
