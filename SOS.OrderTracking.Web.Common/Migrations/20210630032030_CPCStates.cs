using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class CPCStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConsignmentStateType",
                table: "CPCServices",
                newName: "CPCConsignmentDisposalState");

            migrationBuilder.AddColumn<short>(
                name: "CPCConsignmentProcessingState",
                table: "CPCServices",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPCConsignmentProcessingState",
                table: "CPCServices");

            migrationBuilder.RenameColumn(
                name: "CPCConsignmentDisposalState",
                table: "CPCServices",
                newName: "ConsignmentStateType");
        }
    }
}
