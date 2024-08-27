using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RefactorConsignmentStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM ConsignmentStates");
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates");

            migrationBuilder.DropColumn(
                name: "ConsignmentDeliveryId",
                table: "ConsignmentStates");

            migrationBuilder.RenameColumn(
                name: "DeliveryStateType",
                table: "ConsignmentStates",
                newName: "ConsignmentStateType");

            migrationBuilder.RenameColumn(
                name: "ConsignmentStatus",
                table: "Consignments",
                newName: "ConsignmentStateType");

            migrationBuilder.RenameColumn(
                name: "CashCountType",
                table: "ConsignmentDenomination",
                newName: "DenominationType");

            migrationBuilder.DropColumn(
                name: "DeliveryStateType",
                table: "ConsignmentDeliveries");

            migrationBuilder.AddColumn<byte>(
                name: "DeliveryState",
                table: "ConsignmentDeliveries",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates",
                columns: new[] { "ConsignmentId", "ConsignmentStateType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates");

            migrationBuilder.DropColumn(
                name: "ConsignmentStateType",
                table: "ConsignmentStates");

            migrationBuilder.DropColumn(
                name: "ConsignmentStateType",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "DenominationType",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "DeliveryState",
                table: "ConsignmentDeliveries");

            migrationBuilder.AddColumn<int>(
                name: "ConsignmentDeliveryId",
                table: "ConsignmentStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "DeliveryStateType",
                table: "ConsignmentStates",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ConsignmentStatus",
                table: "Consignments",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "CashCountType",
                table: "ConsignmentDenomination",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DeliveryStateType",
                table: "ConsignmentDeliveries",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates",
                columns: new[] { "ConsignmentDeliveryId", "DeliveryStateType" });
        }
    }
}
