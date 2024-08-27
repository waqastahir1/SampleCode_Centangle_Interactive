using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class _75xCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency75x",
                table: "CPCTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer75x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos75x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency75x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency75x",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer75x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos75x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "Currency75x",
                table: "ConsignmentDenomination");
        }
    }
}
