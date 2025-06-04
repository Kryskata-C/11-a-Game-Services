using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Teams",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Teams",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHour",
                table: "Teams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Teams",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewerName",
                table: "Reviews",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CommentText",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reviews",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "b2492a15-9354-4b4b-a24e-a5989e94fa9b", "AQAAAAIAAYagAAAAEO1UN9ILQT0iL3r9tLYrSMhJ6cAOwu4HRnKFU9ihMgOufcZGB8gaXQPli/fYu+IIzQ==" });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/playeruploads/default-player.png");

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/playeruploads/default-player.png");

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/playeruploads/default-player.png");

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "Description", "GamerTag", "ImageUrl", "PricePerHour", "Rating", "TeamId" },
                values: new object[] { 4, "Prefers to work alone, but available.", "SoloStar", "/images/playeruploads/default-player.png", 30.00m, 4.2000000000000002, null });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ReviewDate", "TeamId", "UserId" },
                values: new object[] { new DateTime(2025, 5, 30, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(819), null, null });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ReviewDate", "TeamId", "UserId" },
                values: new object[] { new DateTime(2025, 6, 2, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(825), null, null });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ReviewDate", "TeamId", "UserId" },
                values: new object[] { new DateTime(2025, 6, 1, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(827), null, null });

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "ImageUrl", "PricePerHour", "Rating" },
                values: new object[] { new DateTime(2025, 6, 4, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(769), "/images/default-team.png", 100.00m, 0.0 });

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "ImageUrl", "PricePerHour", "Rating" },
                values: new object[] { new DateTime(2025, 6, 4, 12, 22, 48, 843, DateTimeKind.Utc).AddTicks(775), "/images/default-team.png", 120.00m, 0.0 });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TeamId",
                table: "Reviews",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Teams_TeamId",
                table: "Reviews",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Teams_TeamId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_TeamId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews");

            migrationBuilder.DeleteData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "PricePerHour",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reviews");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Teams",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewerName",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommentText",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f9e8d7c6-b5a4-3333-2222-1111fedcba98",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "ac2ff6df-c2ff-46e8-bcbe-9c50bc9b3884", "AQAAAAIAAYagAAAAEPhCicgCo5N9sF4SByR3QzkLrKJMqzIx0RSw4xnhuvawUazv2tGU8LqXHmaJUy+G7A==" });

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://example.com/images/progamerx.png");

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://example.com/images/strategistsupreme.png");

            migrationBuilder.UpdateData(
                table: "Players",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://example.com/images/newtalent.png");

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 29, 23, 21, 22, 484, DateTimeKind.Utc).AddTicks(3625));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewDate",
                value: new DateTime(2025, 6, 1, 23, 21, 22, 484, DateTimeKind.Utc).AddTicks(3632));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewDate",
                value: new DateTime(2025, 5, 31, 23, 21, 22, 484, DateTimeKind.Utc).AddTicks(3634));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2025, 6, 3, 23, 21, 22, 484, DateTimeKind.Utc).AddTicks(3500));

            migrationBuilder.UpdateData(
                table: "Teams",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2025, 6, 3, 23, 21, 22, 484, DateTimeKind.Utc).AddTicks(3502));

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
