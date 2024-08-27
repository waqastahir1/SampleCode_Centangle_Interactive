using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedDistance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Distance",
                table: "Consignments",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_FromPartyId",
                table: "ConsignmentDeliveries",
                column: "FromPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_ToPartyId",
                table: "ConsignmentDeliveries",
                column: "ToPartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsignmentDeliveries_Parties_FromPartyId",
                table: "ConsignmentDeliveries",
                column: "FromPartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsignmentDeliveries_Parties_ToPartyId",
                table: "ConsignmentDeliveries",
                column: "ToPartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsignmentDeliveries_Parties_FromPartyId",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsignmentDeliveries_Parties_ToPartyId",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_ConsignmentDeliveries_FromPartyId",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_ConsignmentDeliveries_ToPartyId",
                table: "ConsignmentDeliveries");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Consignments");
        }
    }
}
