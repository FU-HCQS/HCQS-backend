using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class fixerrorfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierPriceQuotations_Suppliers_SupllierId",
                table: "SupplierPriceQuotations");

            migrationBuilder.RenameColumn(
                name: "SupllierId",
                table: "SupplierPriceQuotations",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierPriceQuotations_SupllierId",
                table: "SupplierPriceQuotations",
                newName: "IX_SupplierPriceQuotations_SupplierId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "7d55762b-bfcf-4683-aaa9-f6e86a376bcc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "d77cd71f-8ea0-4f46-9bdd-d551e0c582cd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "1b37c5b1-7145-4463-a69a-38516b45d64b");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierPriceQuotations_Suppliers_SupplierId",
                table: "SupplierPriceQuotations",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierPriceQuotations_Suppliers_SupplierId",
                table: "SupplierPriceQuotations");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "SupplierPriceQuotations",
                newName: "SupllierId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierPriceQuotations_SupplierId",
                table: "SupplierPriceQuotations",
                newName: "IX_SupplierPriceQuotations_SupllierId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "cb3c2885-420a-4d31-80c8-8342ac835561");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "464ea0dc-c298-47a5-aa81-cbfe2f27181c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "db23e24d-b8ac-4ce1-8d7e-650f4a12ab67");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierPriceQuotations_Suppliers_SupllierId",
                table: "SupplierPriceQuotations",
                column: "SupllierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}