using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class UserExpiry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Edit-Distance-Approve",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Edit-Distance-Approve", "EDIT-DISTANCE-APPROVE" });

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[] { "SOS-Headoffice-Billing", null, "SOS-Headoffice-Billing", "SOS-HEADOFFICE-BILLING" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "SOS-Headoffice-Billing");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Edit-Distance-Approve",
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Edit-Distance-Admin", "EDIT-DISTANCE-ADMIN" });
        }
    }
}
