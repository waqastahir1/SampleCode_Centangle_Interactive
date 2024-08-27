using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedBillBranchMainCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from [dbo].[DeliveryGeologs];" +
                "Delete from [dbo].[Denominations];" +
                "Delete from [dbo].[DeliverStatuses];" +
                "Delete from [dbo].[ConsignmentSealCodes];" +
                "Delete from [dbo].[ConsignmentDeliveries];" +
                "Delete from [dbo].[Consignments];");

            migrationBuilder.AddColumn<int>(
                name: "BillBranchId",
                table: "Consignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MainCustomerId",
                table: "Consignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Consignments_BillBranchId",
                table: "Consignments",
                column: "BillBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Consignments_MainCustomerId",
                table: "Consignments",
                column: "MainCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consignments_Parties_BillBranchId",
                table: "Consignments",
                column: "BillBranchId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Consignments_Parties_MainCustomerId",
                table: "Consignments",
                column: "MainCustomerId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consignments_Parties_BillBranchId",
                table: "Consignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Consignments_Parties_MainCustomerId",
                table: "Consignments");

            migrationBuilder.DropIndex(
                name: "IX_Consignments_BillBranchId",
                table: "Consignments");

            migrationBuilder.DropIndex(
                name: "IX_Consignments_MainCustomerId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "BillBranchId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "MainCustomerId",
                table: "Consignments");
        }
    }
}
