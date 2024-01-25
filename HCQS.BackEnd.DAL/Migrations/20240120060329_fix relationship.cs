using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCQS.BackEnd.DAL.Migrations
{
    public partial class fixrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaticFiles_SampleProjects_SampleProjectId",
                table: "StaticFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "SampleProjectId",
                table: "StaticFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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
                name: "FK_StaticFiles_SampleProjects_SampleProjectId",
                table: "StaticFiles",
                column: "SampleProjectId",
                principalTable: "SampleProjects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaticFiles_SampleProjects_SampleProjectId",
                table: "StaticFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "SampleProjectId",
                table: "StaticFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1045c37d-e6eb-4be7-a5c3-fdca47a1fe21",
                column: "ConcurrencyStamp",
                value: "80967e13-e26c-45e9-9746-9686e70c62c9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2f28c722-04c9-41fd-85e4-eaa506acda38",
                column: "ConcurrencyStamp",
                value: "cd8bbf22-0928-421b-8d8d-a903641783f6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1c676b-50f6-4b6f-9b7e-f59a0c135c0f",
                column: "ConcurrencyStamp",
                value: "78cb594d-007e-4263-a19a-30273272631a");

            migrationBuilder.AddForeignKey(
                name: "FK_StaticFiles_SampleProjects_SampleProjectId",
                table: "StaticFiles",
                column: "SampleProjectId",
                principalTable: "SampleProjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
