using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class VaultTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CrewOutId",
                table: "VaultedShipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountOut",
                table: "VaultedShipments",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "BagsIn",
                table: "VaultedShipments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BagsOut",
                table: "VaultedShipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVaulted",
                table: "VaultedShipments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BagsIn",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "BagsOut",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "IsVaulted",
                table: "VaultedShipments");

            migrationBuilder.AlterColumn<int>(
                name: "CrewOutId",
                table: "VaultedShipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountOut",
                table: "VaultedShipments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
