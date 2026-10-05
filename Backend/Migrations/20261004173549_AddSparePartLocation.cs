using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHisseler.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSparePartLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "SpareParts",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationAddress",
                table: "SpareParts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "SpareParts",
                type: "REAL",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 87,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 88,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 89,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 90,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 91,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 92,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 93,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 94,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 95,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 96,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 97,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 98,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 99,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "Latitude", "LocationAddress", "Longitude" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5566));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5579));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5581));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5582));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5584));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5585));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5587));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5589));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 35, 48, 561, DateTimeKind.Local).AddTicks(5590));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "LocationAddress",
                table: "SpareParts");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "SpareParts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7169));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7186));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7190));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7193));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7195));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7197));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7200));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7202));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2026, 10, 4, 21, 13, 46, 187, DateTimeKind.Local).AddTicks(7205));
        }
    }
}
