using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RenamedDbSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               name: "FK_ShipmentCharges_ChargesTypes_ChargeTypeId",
               table: "ShipmentCharges");

            migrationBuilder.RenameTable(
                name: "ChargesTypes",
                newName: "ShipmentChargeType");


            migrationBuilder.RenameTable(
               name: "ConsignmentSealCodes",
               newName: "ShipmentSealCodes");


            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentCharges_ShipmentChargeType_ChargeTypeId",
                table: "ShipmentCharges",
                column: "ChargeTypeId",
                principalTable: "ShipmentChargeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentCharges_ShipmentChargeType_ChargeTypeId",
                table: "ShipmentCharges");

            migrationBuilder.DropTable(
                name: "ShipmentChargeType");

            migrationBuilder.DropTable(
                name: "ShipmentSealCodes");

            migrationBuilder.CreateTable(
                name: "ChargesTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargesTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsignmentSealCodes",
                columns: table => new
                {
                    ConsignmentId = table.Column<int>(type: "int", nullable: false),
                    SealCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsignmentSealCodes", x => new { x.ConsignmentId, x.SealCode });
                });

            migrationBuilder.InsertData(
                table: "ChargesTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Base Charges" },
                    { 2, "Surcharge" },
                    { 3, "Additional Charges" },
                    { 4, "Seal Charges" },
                    { 5, "Over Time Charges" },
                    { 6, "Distance Charges" },
                    { 7, "Extra Charges" },
                    { 8, "Waiting Charges" },
                    { 9, "Toll Charges" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentCharges_ChargesTypes_ChargeTypeId",
                table: "ShipmentCharges",
                column: "ChargeTypeId",
                principalTable: "ChargesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
