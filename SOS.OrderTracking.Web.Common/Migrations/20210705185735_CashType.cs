using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class CashType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionFactor",
                table: "CPCTransactions",
                newName: "CashType");

            migrationBuilder.AddColumn<short>(
                name: "CashNature",
                table: "CPCTransactions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashNature",
                table: "CPCTransactions");

            migrationBuilder.RenameColumn(
                name: "CashType",
                table: "CPCTransactions",
                newName: "TransactionFactor");
        }
    }
}
