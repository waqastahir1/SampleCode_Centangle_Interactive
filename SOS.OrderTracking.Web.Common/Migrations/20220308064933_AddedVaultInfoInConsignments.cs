using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedVaultInfoInConsignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVault",
                table: "Consignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "VaultInTime",
                table: "Consignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VaultOutTime",
                table: "Consignments",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVault",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "VaultInTime",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "VaultOutTime",
                table: "Consignments");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Notifications",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
