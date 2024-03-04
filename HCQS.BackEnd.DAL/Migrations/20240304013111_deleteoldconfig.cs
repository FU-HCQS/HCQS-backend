using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class deleteoldconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConstructionConfigs");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "448eead7-0337-4ba2-bc21-6a22ea538d1b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "25003fb1-6a1a-4585-a72c-772a05d7f6e4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "f2405c6e-61fa-4607-8db6-f85b9e6cc135");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructionConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructionConfigs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "ff4ab8b1-b7ce-427f-866d-3750e3b35e13");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "0e7f2c44-5188-42bc-96a7-dca1eb6ee966");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "f5eb5ca7-dce4-4b3a-9921-c1e02c5949c7");
        }
    }
}
