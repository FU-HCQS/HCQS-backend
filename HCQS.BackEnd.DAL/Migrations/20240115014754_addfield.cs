using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class addfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2dc5c289-ee2a-4b65-8242-d5cb2a9f0d7d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a421dd8f-67cb-495f-bee4-9e3b28551e3d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b739a50f-ff0c-4d08-8241-df084cc6167c");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3933425a-0047-4893-8769-eadb19a81fb2", "5faf1349-056c-46ed-8873-41cc8357f87c", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "96b02a76-9192-4574-b8aa-269289941fcc", "b0837f29-deba-409e-880c-c3ec4e6bdd37", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ae60bd68-ffa8-4b1a-9743-eb8b11ffdcd0", "1b64ed38-3902-4975-80b3-97b800251c30", "CUSTOMER", "customer" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3933425a-0047-4893-8769-eadb19a81fb2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "96b02a76-9192-4574-b8aa-269289941fcc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ae60bd68-ffa8-4b1a-9743-eb8b11ffdcd0");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Blogs");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2dc5c289-ee2a-4b65-8242-d5cb2a9f0d7d", "9aded4fd-04cc-4289-a7d7-82d11e53d563", "ADMIN", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a421dd8f-67cb-495f-bee4-9e3b28551e3d", "52395c25-5cb0-442d-aa12-9fe47b6ec543", "STAFF", "staff" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b739a50f-ff0c-4d08-8241-df084cc6167c", "380f6cf7-1665-44df-8e69-90df672a8240", "CUSTOMER", "customer" });
        }
    }
}
