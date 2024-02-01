using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class addFieldProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConstructionType",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "4bb7a6ce-9b42-44ec-877c-f975f767c937");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "13662843-c515-471b-ab7e-d26456659b0e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "05660f2d-eebd-487d-8573-272031ad64f2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConstructionType",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "e28d7c78-1c12-443d-90b0-0bea4382a7db");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "53bdccc9-61a7-4f46-8020-0dd1e622bb96");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "736684aa-529b-4c31-8bb4-861682d213a5");
        }
    }
}
