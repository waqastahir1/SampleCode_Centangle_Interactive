using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class Missing_Shipments_Alter_ShipmentAlike_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MissingShipmentsFromGBMS",
                table: "MissingShipmentsFromGBMS");

            migrationBuilder.RenameTable(
                name: "MissingShipmentsFromGBMS",
                newName: "MissingShipments");

            migrationBuilder.AddColumn<string>(
                name: "SimilarRecordsJson",
                table: "MissingShipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissingShipments",
                table: "MissingShipments",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MissingShipments",
                table: "MissingShipments");

            migrationBuilder.DropColumn(
                name: "SimilarRecordsJson",
                table: "MissingShipments");

            migrationBuilder.RenameTable(
                name: "MissingShipments",
                newName: "MissingShipmentsFromGBMS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MissingShipmentsFromGBMS",
                table: "MissingShipmentsFromGBMS",
                column: "Id");
        }
    }
}
