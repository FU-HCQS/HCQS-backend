using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class fixDB2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConstructionMaterials_ExportPriceMaterials_ExportPriceMaterialId",
                table: "ConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ConstructionMaterials_MaterialHistories_MaterialHistoryId",
                table: "ConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ConstructionMaterials_Projects_ProjectId",
                table: "ConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ConstructionMaterials_QuotationDetails_QuotationDetailId",
                table: "ConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialHistories_Suppliers_SupllierId",
                table: "MaterialHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierPriceDetails_MaterialHistories_SupplierPriceQuotationId",
                table: "SupplierPriceDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialHistories",
                table: "MaterialHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConstructionMaterials",
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

            migrationBuilder.RenameTable(
                name: "MaterialHistories",
                newName: "SupplierPriceQuotations");

            migrationBuilder.RenameTable(
                name: "ConstructionMaterials",
                newName: "ProgressConstructionMaterials");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialHistories_SupllierId",
                table: "SupplierPriceQuotations",
                newName: "IX_SupplierPriceQuotations_SupllierId");

            migrationBuilder.RenameIndex(
                name: "IX_ConstructionMaterials_QuotationDetailId",
                table: "ProgressConstructionMaterials",
                newName: "IX_ProgressConstructionMaterials_QuotationDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_ConstructionMaterials_ProjectId",
                table: "ProgressConstructionMaterials",
                newName: "IX_ProgressConstructionMaterials_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ConstructionMaterials_MaterialHistoryId",
                table: "ProgressConstructionMaterials",
                newName: "IX_ProgressConstructionMaterials_MaterialHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ConstructionMaterials_ExportPriceMaterialId",
                table: "ProgressConstructionMaterials",
                newName: "IX_ProgressConstructionMaterials_ExportPriceMaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierPriceQuotations",
                table: "SupplierPriceQuotations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgressConstructionMaterials",
                table: "ProgressConstructionMaterials",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21", "80967e13-e26c-45e9-9746-9686e70c62c9", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2f28c722-04c9-41fd-85e4-eaa506acda38", "cd8bbf22-0928-421b-8d8d-a903641783f6", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f", "78cb594d-007e-4263-a19a-30273272631a", "CUSTOMER", "customer" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressConstructionMaterials_ExportPriceMaterials_ExportPriceMaterialId",
                table: "ProgressConstructionMaterials",
                column: "ExportPriceMaterialId",
                principalTable: "ExportPriceMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressConstructionMaterials_Projects_ProjectId",
                table: "ProgressConstructionMaterials",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressConstructionMaterials_QuotationDetails_QuotationDetailId",
                table: "ProgressConstructionMaterials",
                column: "QuotationDetailId",
                principalTable: "QuotationDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgressConstructionMaterials_SupplierPriceQuotations_MaterialHistoryId",
                table: "ProgressConstructionMaterials",
                column: "MaterialHistoryId",
                principalTable: "SupplierPriceQuotations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierPriceDetails_SupplierPriceQuotations_SupplierPriceQuotationId",
                table: "SupplierPriceDetails",
                column: "SupplierPriceQuotationId",
                principalTable: "SupplierPriceQuotations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierPriceQuotations_Suppliers_SupllierId",
                table: "SupplierPriceQuotations",
                column: "SupllierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgressConstructionMaterials_ExportPriceMaterials_ExportPriceMaterialId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgressConstructionMaterials_Projects_ProjectId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgressConstructionMaterials_QuotationDetails_QuotationDetailId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgressConstructionMaterials_SupplierPriceQuotations_MaterialHistoryId",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierPriceDetails_SupplierPriceQuotations_SupplierPriceQuotationId",
                table: "SupplierPriceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierPriceQuotations_Suppliers_SupllierId",
                table: "SupplierPriceQuotations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierPriceQuotations",
                table: "SupplierPriceQuotations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgressConstructionMaterials",
                table: "ProgressConstructionMaterials");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f");

            migrationBuilder.RenameTable(
                name: "SupplierPriceQuotations",
                newName: "MaterialHistories");

            migrationBuilder.RenameTable(
                name: "ProgressConstructionMaterials",
                newName: "ConstructionMaterials");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierPriceQuotations_SupllierId",
                table: "MaterialHistories",
                newName: "IX_MaterialHistories_SupllierId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgressConstructionMaterials_QuotationDetailId",
                table: "ConstructionMaterials",
                newName: "IX_ConstructionMaterials_QuotationDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgressConstructionMaterials_ProjectId",
                table: "ConstructionMaterials",
                newName: "IX_ConstructionMaterials_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgressConstructionMaterials_MaterialHistoryId",
                table: "ConstructionMaterials",
                newName: "IX_ConstructionMaterials_MaterialHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgressConstructionMaterials_ExportPriceMaterialId",
                table: "ConstructionMaterials",
                newName: "IX_ConstructionMaterials_ExportPriceMaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialHistories",
                table: "MaterialHistories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConstructionMaterials",
                table: "ConstructionMaterials",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ConstructionMaterials_ExportPriceMaterials_ExportPriceMaterialId",
                table: "ConstructionMaterials",
                column: "ExportPriceMaterialId",
                principalTable: "ExportPriceMaterials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConstructionMaterials_MaterialHistories_MaterialHistoryId",
                table: "ConstructionMaterials",
                column: "MaterialHistoryId",
                principalTable: "MaterialHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ConstructionMaterials_Projects_ProjectId",
                table: "ConstructionMaterials",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConstructionMaterials_QuotationDetails_QuotationDetailId",
                table: "ConstructionMaterials",
                column: "QuotationDetailId",
                principalTable: "QuotationDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialHistories_Suppliers_SupllierId",
                table: "MaterialHistories",
                column: "SupllierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierPriceDetails_MaterialHistories_SupplierPriceQuotationId",
                table: "SupplierPriceDetails",
                column: "SupplierPriceQuotationId",
                principalTable: "MaterialHistories",
                principalColumn: "Id");
        }
    }
}