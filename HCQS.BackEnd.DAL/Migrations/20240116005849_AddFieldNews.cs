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
                values: new object[] { "0ecef207-4829-458c-9bc9-738a8f7aa47e", "e8d04dc1-15a9-4fd5-a95b-06124fea8531", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "86a13318-21b1-4263-b53c-9f102f7c7654", "a5516b39-a826-42b5-b1ae-6de3b4a438f8", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "eadf04bf-8b44-459a-970e-8400b800a071", "72be4c6b-b64f-42e2-9911-621b748cfa36", "CUSTOMER", "customer" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0ecef207-4829-458c-9bc9-738a8f7aa47e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "86a13318-21b1-4263-b53c-9f102f7c7654");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eadf04bf-8b44-459a-970e-8400b800a071");

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
