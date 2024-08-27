using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class Missing_Shipments_Alter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClearedShipments");

            migrationBuilder.CreateTable(
                name: "MissingShipmentsFromGBMS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentNumberProcessed = table.Column<double>(type: "float", nullable: false),
                    PreviousShipmentNumberProcessed = table.Column<double>(type: "float", nullable: false),
                    PreviousShipmentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentPickup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentDropOff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextShipmentNumberProcessed = table.Column<double>(type: "float", nullable: false),
                    NextShipmentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentPickup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentDropOff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissingShipmentsFromGBMS", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissingShipmentsFromGBMS");

            migrationBuilder.CreateTable(
                name: "ClearedShipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<int>(type: "int", nullable: false),
                    NextShipmentCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NextShipmentDropOff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextShipmentNumberProcessed = table.Column<double>(type: "float", nullable: false),
                    NextShipmentPickup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousShipmentDropOff = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousShipmentNumberProcessed = table.Column<double>(type: "float", nullable: false),
                    PreviousShipmentPickup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    To = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClearedShipments", x => x.Id);
                });
        }
    }
}
