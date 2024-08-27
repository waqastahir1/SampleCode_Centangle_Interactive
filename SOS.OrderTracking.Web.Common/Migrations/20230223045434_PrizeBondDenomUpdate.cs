using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class PrizeBondDenomUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney100x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney15000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney1500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney200x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney25000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney40000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney7500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney750x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney100x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney15000x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney1500x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney200x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney25000x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney40000x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney7500x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrizeMoney750x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrizeMoney100x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney15000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney1500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney200x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney25000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney40000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney7500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney750x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "PrizeMoney100x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney15000x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney1500x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney200x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney25000x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney40000x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney7500x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "PrizeMoney750x",
                table: "ConsignmentDenomination");
        }
    }
}
