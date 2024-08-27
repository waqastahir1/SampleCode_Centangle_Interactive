using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class VaultedShipmentAlterForManualEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                table: "VaultedShipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CrewInId",
                table: "VaultedShipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FromBranchId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ManualShipmentId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToBranchId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleInId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleOutId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromBranchId",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "ManualShipmentId",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "ToBranchId",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "VehicleInId",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "VehicleOutId",
                table: "VaultedShipments");

            migrationBuilder.AlterColumn<int>(
                name: "ShipmentId",
                table: "VaultedShipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CrewInId",
                table: "VaultedShipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
