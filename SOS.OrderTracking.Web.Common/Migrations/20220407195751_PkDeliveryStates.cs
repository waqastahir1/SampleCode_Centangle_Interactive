using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class PkDeliveryStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates",
                columns: new[] { "ConsignmentId", "DeliveryId", "ConsignmentStateType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsignmentStates",
                table: "ConsignmentStates",
                columns: new[] { "ConsignmentId", "ConsignmentStateType" });
        }
    }
}
