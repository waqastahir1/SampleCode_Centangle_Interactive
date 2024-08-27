using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class OriginAndCounterParties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CounterPartyId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginPartyId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "ShipmentApprovalMode",
                table: "Consignments",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CounterPartyId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "OriginPartyId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "ShipmentApprovalMode",
                table: "Consignments");
        }
    }
}
