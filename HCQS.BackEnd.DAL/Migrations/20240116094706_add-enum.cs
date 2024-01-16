using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class addenum : Migration
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

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StaticFileType",
                table: "StaticFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuotationStatus",
                table: "Quotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectStatus",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeResponse",
                table: "PaymentResponses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "News",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaterialType",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitMaterial",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3e0d3948-6886-465f-8492-cc29df635ce7", "c1b86dbe-15a4-45da-8298-0d30b6842d7b", "CUSTOMER", "customer" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5dab1591-9690-4d3d-ba4f-71ab966fd85a", "5724fb95-b401-4096-a7cc-e05915836f48", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8677f9cb-f950-468a-9e56-69c589637674", "6bf790e4-bd11-434a-a385-c27d722c0fd1", "STAFF", "staff" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3e0d3948-6886-465f-8492-cc29df635ce7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5dab1591-9690-4d3d-ba4f-71ab966fd85a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8677f9cb-f950-468a-9e56-69c589637674");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "StaticFileType",
                table: "StaticFiles");

            migrationBuilder.DropColumn(
                name: "QuotationStatus",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "ProjectStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentTypeResponse",
                table: "PaymentResponses");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "News");

            migrationBuilder.DropColumn(
                name: "MaterialType",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "UnitMaterial",
                table: "Materials");

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
