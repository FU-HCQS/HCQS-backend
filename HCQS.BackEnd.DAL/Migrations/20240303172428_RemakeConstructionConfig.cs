using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class RemakeConstructionConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructionConfigValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SandMixingRatio = table.Column<double>(type: "float", nullable: false),
                    CementMixingRatio = table.Column<double>(type: "float", nullable: false),
                    StoneMixingRatio = table.Column<double>(type: "float", nullable: false),
                    ConstructionType = table.Column<int>(type: "int", nullable: false),
                    NumOfFloorMin = table.Column<int>(type: "int", nullable: false),
                    NumOfFloorMax = table.Column<int>(type: "int", nullable: true),
                    AreaMin = table.Column<int>(type: "int", nullable: false),
                    AreaMax = table.Column<int>(type: "int", nullable: true),
                    TiledAreaMin = table.Column<int>(type: "int", nullable: false),
                    TiledAreaMax = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructionConfigValues", x => x.Id);
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConstructionConfigValues");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "62e107d1-250b-4595-b473-c98aa7d34f3d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "21958a14-e0f5-467f-be11-3814d23219e7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "7f6bad0e-adb2-4938-8122-0c9a8da570d0");
        }
    }
}