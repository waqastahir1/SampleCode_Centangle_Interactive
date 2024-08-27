using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class ATMRDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CITDropoffQrCode",
                table: "ATMServices",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CITPickupQrCode",
                table: "ATMServices",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupQrCode",
                table: "ATMServices",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnCITPickupQrCode",
                table: "ATMServices",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FCMToken",
                table: "AspNetUsers",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ATMRSealCodes",
                columns: table => new
                {
                    AtmrServiceId = table.Column<int>(nullable: false),
                    SealCode = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMRSealCodes", x => new { x.AtmrServiceId, x.SealCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ATMRSealCodes");

            migrationBuilder.DropColumn(
                name: "CITDropoffQrCode",
                table: "ATMServices");

            migrationBuilder.DropColumn(
                name: "CITPickupQrCode",
                table: "ATMServices");

            migrationBuilder.DropColumn(
                name: "PickupQrCode",
                table: "ATMServices");

            migrationBuilder.DropColumn(
                name: "ReturnCITPickupQrCode",
                table: "ATMServices");

            migrationBuilder.DropColumn(
                name: "FCMToken",
                table: "AspNetUsers");
        }
    }
}
