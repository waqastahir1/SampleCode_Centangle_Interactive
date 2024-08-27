using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class ShipmentChargesRedefined : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Waiting Charges");

            migrationBuilder.UpdateData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Toll Charges");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Base Charges");

            migrationBuilder.UpdateData(
                table: "ShipmentChargeType",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Surcharge");

            migrationBuilder.InsertData(
                table: "ShipmentChargeType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Additional Charges" },
                    { 4, "Seal Charges" },
                    { 5, "Over Time Charges" },
                    { 6, "Distance Charges" },
                    { 7, "Extra Charges" },
                    { 8, "Waiting Charges" },
                    { 9, "Toll Charges" }
                });
        }
    }
}
