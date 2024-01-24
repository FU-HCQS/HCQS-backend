using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class changeDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressConstructionMaterials_SupplierPriceQuotations_MaterialHistoryId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DropIndex(
                name: "IX_ProgressConstructionMaterials_MaterialHistoryId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DropColumn(
                name: "MaterialHistoryId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "QuotationDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProgressConstructionMaterialId",
                table: "ImportExportInventoryHistorys",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "e1461298-f779-4cb6-b240-ece35a24bebb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "78fa3455-ced3-4b94-b000-b452e8926ee0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "a5c9a11e-937a-4233-abab-94b4866b1914");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_MaterialId",
                table: "QuotationDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportExportInventoryHistorys_ProgressConstructionMaterialId",
                table: "ImportExportInventoryHistorys",
                column: "ProgressConstructionMaterialId",
                unique: true,
                filter: "[ProgressConstructionMaterialId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportExportInventoryHistorys_ProgressConstructionMaterials_ProgressConstructionMaterialId",
                table: "ImportExportInventoryHistorys",
                column: "ProgressConstructionMaterialId",
                principalTable: "ProgressConstructionMaterials",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_Materials_MaterialId",
                table: "QuotationDetails",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportExportInventoryHistorys_ProgressConstructionMaterials_ProgressConstructionMaterialId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_Materials_MaterialId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_MaterialId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_ImportExportInventoryHistorys_ProgressConstructionMaterialId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "ProgressConstructionMaterialId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialHistoryId",
                table: "ProgressConstructionMaterials",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "1510328d-7712-4a64-8982-b6f29a4a992e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "ce090292-dbfe-4ad2-bebf-ebabacd3ee0c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "dacba891-73bd-4d67-900b-15e0ae1608db");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressConstructionMaterials_MaterialHistoryId",
                table: "ProgressConstructionMaterials",
                column: "MaterialHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressConstructionMaterials_SupplierPriceQuotations_MaterialHistoryId",
                table: "ProgressConstructionMaterials",
                column: "MaterialHistoryId",
                principalTable: "SupplierPriceQuotations",
                principalColumn: "Id");
        }
    }
}
