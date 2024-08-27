using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class DeliveryGeolocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "CollectionPoint",
                table: "ConsignmentDeliveries",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CollectionPointStatus",
                table: "ConsignmentDeliveries",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<Point>(
                name: "DeliveryPoint",
                table: "ConsignmentDeliveries",
                type: "geography",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "DeliveryPointStatus",
                table: "ConsignmentDeliveries",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionPoint",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "CollectionPointStatus",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryPoint",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryPointStatus",
                table: "ConsignmentDeliveries");

        }
    }
}
