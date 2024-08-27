using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class FinalizeShipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JsonData",
                table: "DedicatedVehiclesCapacities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RadiusInKm",
                table: "DedicatedVehiclesCapacities",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TripPerDay",
                table: "DedicatedVehiclesCapacities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinalizedAt",
                table: "Consignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalizedBy",
                table: "Consignments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "Consignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NoOfBags",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonData",
                table: "DedicatedVehiclesCapacities");

            migrationBuilder.DropColumn(
                name: "RadiusInKm",
                table: "DedicatedVehiclesCapacities");

            migrationBuilder.DropColumn(
                name: "TripPerDay",
                table: "DedicatedVehiclesCapacities");

            migrationBuilder.DropColumn(
                name: "FinalizedAt",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "FinalizedBy",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "NoOfBags",
                table: "Consignments");
        }
    }
}
