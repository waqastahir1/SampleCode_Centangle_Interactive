using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class CpcAmounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectsCustomerBalance",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "EffectsProcessedBalance",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "FromPartyId",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "ProcessedBalance",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "ToPartyId",
                table: "CPCTransactions");

            migrationBuilder.RenameColumn(
                name: "UnprocessedBalance",
                table: "CPCTransactions",
                newName: "DrPartyId");

            migrationBuilder.RenameColumn(
                name: "TotalBalance",
                table: "CPCTransactions",
                newName: "CrPartyId");

            migrationBuilder.RenameColumn(
                name: "CPCTransactionType",
                table: "CPCTransactions",
                newName: "CPCTransactionReason");

            migrationBuilder.AddColumn<int>(
                name: "Balance",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DisposedAmount",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedAmount",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedBalance",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnprocessedBalance",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "DisposedAmount",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "ProcessedAmount",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "ProcessedBalance",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "UnprocessedBalance",
                table: "CPCServices");

            migrationBuilder.RenameColumn(
                name: "DrPartyId",
                table: "CPCTransactions",
                newName: "UnprocessedBalance");

            migrationBuilder.RenameColumn(
                name: "CrPartyId",
                table: "CPCTransactions",
                newName: "TotalBalance");

            migrationBuilder.RenameColumn(
                name: "CPCTransactionReason",
                table: "CPCTransactions",
                newName: "CPCTransactionType");

            migrationBuilder.AddColumn<bool>(
                name: "EffectsCustomerBalance",
                table: "CPCTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EffectsProcessedBalance",
                table: "CPCTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FromPartyId",
                table: "CPCTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "CPCTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedBalance",
                table: "CPCTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToPartyId",
                table: "CPCTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
