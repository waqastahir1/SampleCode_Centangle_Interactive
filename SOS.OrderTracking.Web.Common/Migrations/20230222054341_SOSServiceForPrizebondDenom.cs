using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class SOSServiceForPrizebondDenom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer15000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer1500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer200x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer25000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer40000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer7500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer750x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos15000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos1500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos200x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos25000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos40000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos7500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos750x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer15000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer1500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer200x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer25000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer40000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer7500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer750x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos15000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos1500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos200x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos25000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos40000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos7500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos750x",
                table: "CPCServices");
        }
    }
}
