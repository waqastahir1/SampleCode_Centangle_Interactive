using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RenamedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryCharges_ChargesTypes_ChargeTypeId",
                table: "DeliveryCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryCharges_ConsignmentDeliveries_OrderFulfilmentId",
                table: "DeliveryCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_Denominations_Consignments_ConsignmentId",
                table: "Denominations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Denominations",
                table: "Denominations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryCharges",
                table: "DeliveryCharges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliverStatuses",
                table: "DeliverStatuses");

            migrationBuilder.RenameTable(
                name: "Denominations",
                newName: "ConsignmentDenomination");

            migrationBuilder.RenameTable(
                name: "DeliveryCharges",
                newName: "ShipmentCharges");

            migrationBuilder.RenameTable(
                name: "DeliverStatuses",
                newName: "ConsignmentStates");

            migrationBuilder.RenameIndex(
                name: "IX_Denominations_ConsignmentId",
                table: "ConsignmentDenomination",
                newName: "IX_ConsignmentDenomination_ConsignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryCharges_ChargeTypeId",
                table: "ShipmentCharges",
                newName: "IX_ShipmentCharges_ChargeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsignmentDenomination",
                table: "ConsignmentDenomination",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentCharges",
                table: "ShipmentCharges",
                columns: new[] { "OrderFulfilmentId", "ChargeTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates",
                columns: new[] { "ConsignmentDeliveryId", "DeliveryStateType" });

            migrationBuilder.AddForeignKey(
                name: "FK_ConsignmentDenomination_Consignments_ConsignmentId",
                table: "ConsignmentDenomination",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentCharges_ChargesTypes_ChargeTypeId",
                table: "ShipmentCharges",
                column: "ChargeTypeId",
                principalTable: "ChargesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentCharges_ConsignmentDeliveries_OrderFulfilmentId",
                table: "ShipmentCharges",
                column: "OrderFulfilmentId",
                principalTable: "ConsignmentDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsignmentDenomination_Consignments_ConsignmentId",
                table: "ConsignmentDenomination");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentCharges_ChargesTypes_ChargeTypeId",
                table: "ShipmentCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentCharges_ConsignmentDeliveries_OrderFulfilmentId",
                table: "ShipmentCharges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentCharges",
                table: "ShipmentCharges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsignmentDenomination",
                table: "ConsignmentDenomination");

            migrationBuilder.RenameTable(
                name: "ShipmentCharges",
                newName: "DeliveryCharges");

            migrationBuilder.RenameTable(
                name: "ConsignmentStates",
                newName: "DeliverStatuses");

            migrationBuilder.RenameTable(
                name: "ConsignmentDenomination",
                newName: "Denominations");

            migrationBuilder.RenameIndex(
                name: "IX_ShipmentCharges_ChargeTypeId",
                table: "DeliveryCharges",
                newName: "IX_DeliveryCharges_ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsignmentDenomination_ConsignmentId",
                table: "Denominations",
                newName: "IX_Denominations_ConsignmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryCharges",
                table: "DeliveryCharges",
                columns: new[] { "OrderFulfilmentId", "ChargeTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliverStatuses",
                table: "DeliverStatuses",
                columns: new[] { "ConsignmentDeliveryId", "DeliveryStateType" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Denominations",
                table: "Denominations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryCharges_ChargesTypes_ChargeTypeId",
                table: "DeliveryCharges",
                column: "ChargeTypeId",
                principalTable: "ChargesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryCharges_ConsignmentDeliveries_OrderFulfilmentId",
                table: "DeliveryCharges",
                column: "OrderFulfilmentId",
                principalTable: "ConsignmentDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Denominations_Consignments_ConsignmentId",
                table: "Denominations",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
