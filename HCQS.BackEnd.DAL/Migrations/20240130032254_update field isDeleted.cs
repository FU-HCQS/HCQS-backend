using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class updatefieldisDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkerPrices",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkerPrices");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "24aa4ec5-b6aa-4270-b0ae-fde39fde1704");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "4a5aa0a7-7e27-4f09-92bd-98cca7915c0c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "4d616dd2-953d-48ab-bb30-b53dff5bdd97");
        }
    }
}
