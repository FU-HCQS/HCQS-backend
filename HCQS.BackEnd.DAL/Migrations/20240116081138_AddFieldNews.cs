using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class AddFieldNews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11d3bb47-03c2-4031-b1ec-cac596c607ed");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3faaa2cd-6a5d-4d51-96f3-b554bfd0f999");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "716daafa-ad8c-4a96-95da-f7f277129619");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "News",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "News",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "39fd00dc-6e10-43fb-8d34-4f7293e10f21", "4ec960c2-ef0e-4770-b8a0-4fb3461a5f7d", "CUSTOMER", "customer" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7bf812ec-db04-49c9-8e1d-cb73d8ba7a20", "2945d832-4c90-4cd3-b3d4-623e85ad9c4c", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a049f8cc-f6f7-4996-bb99-35cc122b2f58", "9eb8d34c-2d0f-4b54-9ef9-5f1ead0b74da", "STAFF", "staff" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39fd00dc-6e10-43fb-8d34-4f7293e10f21");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7bf812ec-db04-49c9-8e1d-cb73d8ba7a20");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a049f8cc-f6f7-4996-bb99-35cc122b2f58");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "News");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "News",
                newName: "Image");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "11d3bb47-03c2-4031-b1ec-cac596c607ed", "c12e6606-bff6-4967-a3e8-e5f9aca15327", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3faaa2cd-6a5d-4d51-96f3-b554bfd0f999", "e8e21d2b-1066-4c8c-ac2a-aabaa526a35e", "CUSTOMER", "customer" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "716daafa-ad8c-4a96-95da-f7f277129619", "ba340c44-d2e7-435a-8ac3-ee797a437522", "STAFF", "staff" });
        }
    }
}
