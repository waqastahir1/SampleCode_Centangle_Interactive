using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class UpdateDeliveryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVault",
                table: "ConsignmentDeliveries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "SerialNo",
                table: "ConsignmentDeliveries",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "TemporalState",
                table: "ConsignmentDeliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVault",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "SerialNo",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "TemporalState",
                table: "ConsignmentDeliveries");
        }
    }
}
