using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RecursiveDeliveries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ShipmentCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentCharges_ConsignmentDeliveries_OrderFulfilmentId",
                table: "ShipmentCharges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentCharges",
                table: "ShipmentCharges");

            migrationBuilder.DropColumn(
                name: "OrderFulfilmentId",
                table: "ShipmentCharges");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "ConsignmentDeliveries",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentCharges",
                table: "ShipmentCharges",
                columns: new[] { "ConsignmentId", "ChargeTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_ParentId",
                table: "ConsignmentDeliveries",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsignmentDeliveries_ConsignmentDeliveries_ParentId",
                table: "ConsignmentDeliveries",
                column: "ParentId",
                principalTable: "ConsignmentDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentCharges_Consignments_ConsignmentId",
                table: "ShipmentCharges",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsignmentDeliveries_ConsignmentDeliveries_ParentId",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentCharges_Consignments_ConsignmentId",
                table: "ShipmentCharges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentCharges",
                table: "ShipmentCharges");

            migrationBuilder.DropIndex(
                name: "IX_ConsignmentDeliveries_ParentId",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ConsignmentDeliveries");

            migrationBuilder.AddColumn<int>(
                name: "OrderFulfilmentId",
                table: "ShipmentCharges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentCharges",
                table: "ShipmentCharges",
                columns: new[] { "OrderFulfilmentId", "ChargeTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentCharges_ConsignmentDeliveries_OrderFulfilmentId",
                table: "ShipmentCharges",
                column: "OrderFulfilmentId",
                principalTable: "ConsignmentDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
