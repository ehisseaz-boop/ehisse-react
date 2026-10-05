using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EHisseler.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformedByUserId",
                table: "SparePartSaleAudits",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUpdateSubmission",
                table: "SpareParts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 16,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 17,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 18,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 19,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 20,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 21,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 22,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 23,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 24,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 25,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 26,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 27,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 28,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 29,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 30,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 31,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 32,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 33,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 34,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 35,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 36,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 37,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 38,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 39,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 40,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 41,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 42,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 43,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 44,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 45,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 46,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 47,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 48,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 49,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 50,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 51,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 52,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 53,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 54,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 55,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 56,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 57,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 58,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 59,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 60,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 61,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 62,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 63,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 64,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 65,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 66,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 67,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 68,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 69,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 70,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 71,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 72,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 73,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 74,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 75,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 76,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 77,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 78,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 79,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 80,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 81,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 82,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 83,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 84,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 85,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 86,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 87,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 88,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 89,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 90,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 91,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 92,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 93,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 94,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 95,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 96,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 97,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 98,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 99,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "SpareParts",
                keyColumn: "Id",
                keyValue: 100,
                column: "IsUpdateSubmission",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(397), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(411), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(416), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(419), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(422), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(425), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(431), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(433), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreatedDate", "RejectionReason" },
                values: new object[] { new DateTime(2026, 10, 5, 17, 42, 3, 365, DateTimeKind.Local).AddTicks(436), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PerformedByUserId",
                table: "SparePartSaleAudits");

            migrationBuilder.DropColumn(
                name: "IsUpdateSubmission",
                table: "SpareParts");

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
    }
}
