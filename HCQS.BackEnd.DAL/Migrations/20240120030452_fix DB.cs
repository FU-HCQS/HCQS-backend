using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class fixDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImportExportInventoryHistorys_MaterialHistories_MaterialHistoryId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialHistories_MaterialSuppliers_MaterialSupplierId",
                table: "MaterialHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationDetails_ConstructionMaterials_ConstructionMaterialId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_QuotationDetails_ConstructionMaterialId",
                table: "QuotationDetails");

            migrationBuilder.DropIndex(
                name: "IX_ImportExportInventoryHistorys_MaterialHistoryId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9faf411-0d6f-40b5-818b-a75ea1ac9406");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbc3e3d2-7866-4370-8853-066d732ee80a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4be700b-16d1-499e-8ddc-f7b5da3261b0");

            migrationBuilder.DropColumn(
                name: "ConstructionMaterialId",
                table: "QuotationDetails");

            migrationBuilder.DropColumn(
                name: "ImportPrice",
                table: "MaterialHistories");

            migrationBuilder.DropColumn(
                name: "MaterialHistoryId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.RenameColumn(
                name: "MaterialSupplierId",
                table: "MaterialHistories",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialHistories_MaterialSupplierId",
                table: "MaterialHistories",
                newName: "IX_MaterialHistories_SupplierId");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "SampleProjects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierPriceDetailId",
                table: "ImportExportInventoryHistorys",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ConstructionMaterials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "QuotationDetailId",
                table: "ConstructionMaterials",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SupplierPriceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MOQ = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupplierPriceQuotationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPriceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPriceDetails_MaterialHistories_SupplierPriceQuotationId",
                        column: x => x.SupplierPriceQuotationId,
                        principalTable: "MaterialHistories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierPriceDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6be54b9f-c492-4973-b37b-2a374bacd0a5", "e94620de-87a7-4e02-bd94-20e2d67bc767", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "828b7fe7-8ae6-45f0-bcb8-46a3e3cd4808", "303c4373-c5f6-4e9e-a11d-636c6393a491", "CUSTOMER", "customer" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e4515974-db46-438a-8d4f-473459e4e1c7", "8c0236af-d52f-40c7-b4b7-3c383a89d6f2", "ADMIN", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_SampleProjects_AccountId",
                table: "SampleProjects",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportExportInventoryHistorys_SupplierPriceDetailId",
                table: "ImportExportInventoryHistorys",
                column: "SupplierPriceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ConstructionMaterials_QuotationDetailId",
                table: "ConstructionMaterials",
                column: "QuotationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPriceDetails_MaterialId",
                table: "SupplierPriceDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPriceDetails_SupplierPriceQuotationId",
                table: "SupplierPriceDetails",
                column: "SupplierPriceQuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConstructionMaterials_QuotationDetails_QuotationDetailId",
                table: "ConstructionMaterials",
                column: "QuotationDetailId",
                principalTable: "QuotationDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportExportInventoryHistorys_SupplierPriceDetails_SupplierPriceDetailId",
                table: "ImportExportInventoryHistorys",
                column: "SupplierPriceDetailId",
                principalTable: "SupplierPriceDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialHistories_Suppliers_SupplierId",
                table: "MaterialHistories",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SampleProjects_AspNetUsers_AccountId",
                table: "SampleProjects",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConstructionMaterials_QuotationDetails_QuotationDetailId",
                table: "ConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportExportInventoryHistorys_SupplierPriceDetails_SupplierPriceDetailId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialHistories_Suppliers_SupplierId",
                table: "MaterialHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SampleProjects_AspNetUsers_AccountId",
                table: "SampleProjects");

            migrationBuilder.DropTable(
                name: "SupplierPriceDetails");

            migrationBuilder.DropIndex(
                name: "IX_SampleProjects_AccountId",
                table: "SampleProjects");

            migrationBuilder.DropIndex(
                name: "IX_ImportExportInventoryHistorys_SupplierPriceDetailId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DropIndex(
                name: "IX_ConstructionMaterials_QuotationDetailId",
                table: "ConstructionMaterials");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6be54b9f-c492-4973-b37b-2a374bacd0a5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "828b7fe7-8ae6-45f0-bcb8-46a3e3cd4808");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e4515974-db46-438a-8d4f-473459e4e1c7");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "SampleProjects");

            migrationBuilder.DropColumn(
                name: "SupplierPriceDetailId",
                table: "ImportExportInventoryHistorys");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ConstructionMaterials");

            migrationBuilder.DropColumn(
                name: "QuotationDetailId",
                table: "ConstructionMaterials");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "MaterialHistories",
                newName: "MaterialSupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialHistories_SupplierId",
                table: "MaterialHistories",
                newName: "IX_MaterialHistories_MaterialSupplierId");

            migrationBuilder.AddColumn<Guid>(
                name: "ConstructionMaterialId",
                table: "QuotationDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ImportPrice",
                table: "MaterialHistories",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "MaterialHistoryId",
                table: "ImportExportInventoryHistorys",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c9faf411-0d6f-40b5-818b-a75ea1ac9406", "9f70af7e-e982-4aa9-86b3-7e6cba34c559", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "cbc3e3d2-7866-4370-8853-066d732ee80a", "660459fc-837f-4372-8a58-48d3ed48a7c9", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "e4be700b-16d1-499e-8ddc-f7b5da3261b0", "2af8e291-246e-42a3-8eb4-b269a17aa63c", "CUSTOMER", "customer" });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_ConstructionMaterialId",
                table: "QuotationDetails",
                column: "ConstructionMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportExportInventoryHistorys_MaterialHistoryId",
                table: "ImportExportInventoryHistorys",
                column: "MaterialHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImportExportInventoryHistorys_MaterialHistories_MaterialHistoryId",
                table: "ImportExportInventoryHistorys",
                column: "MaterialHistoryId",
                principalTable: "MaterialHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialHistories_MaterialSuppliers_MaterialSupplierId",
                table: "MaterialHistories",
                column: "MaterialSupplierId",
                principalTable: "MaterialSuppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationDetails_ConstructionMaterials_ConstructionMaterialId",
                table: "QuotationDetails",
                column: "ConstructionMaterialId",
                principalTable: "ConstructionMaterials",
                principalColumn: "Id");
        }
    }
}
