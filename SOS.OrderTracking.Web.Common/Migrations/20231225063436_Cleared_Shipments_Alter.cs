using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class Cleared_Shipments_Alter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "To",
                table: "ClearedShipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "NextShipmentCity",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextShipmentDate",
                table: "ClearedShipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NextShipmentDropOff",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextShipmentNumber",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NextShipmentNumberProcessed",
                table: "ClearedShipments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "NextShipmentPickup",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousShipmentCity",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PreviousShipmentDate",
                table: "ClearedShipments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PreviousShipmentDropOff",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousShipmentNumber",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PreviousShipmentNumberProcessed",
                table: "ClearedShipments",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PreviousShipmentPickup",
                table: "ClearedShipments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextShipmentCity",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "NextShipmentDate",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "NextShipmentDropOff",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "NextShipmentNumber",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "NextShipmentNumberProcessed",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "NextShipmentPickup",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "PreviousShipmentCity",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "PreviousShipmentDate",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "PreviousShipmentDropOff",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "PreviousShipmentNumber",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "PreviousShipmentNumberProcessed",
                table: "ClearedShipments");

            migrationBuilder.DropColumn(
                name: "PreviousShipmentPickup",
                table: "ClearedShipments");

            migrationBuilder.AlterColumn<int>(
                name: "To",
                table: "ClearedShipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
