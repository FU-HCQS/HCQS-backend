using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class addTableContractverificationCodedeleteverficationcodeinAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractVerifyCode",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ContractVerificationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerficationCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractVerificationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractVerificationCodes_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "cade6da0-69c0-4bd6-9cda-e865cd134043");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "55eac941-88f5-489a-ae90-7950d6d36475");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "4a7bf8f2-3b33-46e7-8527-cd917eb317b6");

            migrationBuilder.CreateIndex(
                name: "IX_ContractVerificationCodes_ContractId",
                table: "ContractVerificationCodes",
                column: "ContractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractVerificationCodes");

            migrationBuilder.AddColumn<string>(
                name: "ContractVerifyCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "e5285bd0-a9dd-4c1b-a9b8-b4627fce09d6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "87f752e9-c1f4-4fa9-8a68-81b2d6e8e3b1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "4cc80e17-f9f2-4738-b2b3-bc6339bea7b5");
        }
    }
}