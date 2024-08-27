using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class QRScanAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CollectionMode",
                table: "ConsignmentDeliveries",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DeliveryMode",
                table: "ConsignmentDeliveries",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionMode",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryMode",
                table: "ConsignmentDeliveries");
        }
    }
}
