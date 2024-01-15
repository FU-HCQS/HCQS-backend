using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class customizefield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "669a806c-b60b-4254-b9d4-f64ae873ddc4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "701c774a-cf9d-49a0-a898-15524d9c481f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9c00cff6-88e7-4164-9650-927e2cd063a2");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "669a806c-b60b-4254-b9d4-f64ae873ddc4", "9314d516-2fd4-4e20-9f9c-d8f089e1b4d2", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "701c774a-cf9d-49a0-a898-15524d9c481f", "43d02958-cb3a-4a4c-955f-8dcbfb966826", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9c00cff6-88e7-4164-9650-927e2cd063a2", "07ce6ecd-420a-428d-8d49-0ad2c7cccd14", "CUSTOMER", "customer" });
        }
    }
}
