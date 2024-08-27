using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class VaultedShipmentAlterForChiefCrew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChiefCrewInId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChiefCrewOutId",
                table: "VaultedShipments",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChiefCrewInId",
                table: "VaultedShipments");

            migrationBuilder.DropColumn(
                name: "ChiefCrewOutId",
                table: "VaultedShipments");
        }
    }
}
