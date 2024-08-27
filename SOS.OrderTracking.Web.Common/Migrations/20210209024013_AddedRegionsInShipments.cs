using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedRegionsInShipments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillingRegionId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillingStationId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BillingSubRegionId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollectionRegionId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollectionStationId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CollectionSubRegionId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryRegionId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryStationId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeliverySubRegionId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SealedBags",
                table: "Consignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingRegionId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "BillingStationId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "BillingSubRegionId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "CollectionRegionId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "CollectionStationId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "CollectionSubRegionId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "DeliveryRegionId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "DeliveryStationId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "DeliverySubRegionId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "SealedBags",
                table: "Consignments");
        }
    }
}
