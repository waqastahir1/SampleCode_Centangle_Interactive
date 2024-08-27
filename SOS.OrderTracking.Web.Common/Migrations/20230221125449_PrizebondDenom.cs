using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class PrizebondDenom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Currency15000x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency1500x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency200x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency25000x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency40000x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency7500x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency750x",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency15000x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "Currency1500x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "Currency200x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "Currency25000x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "Currency40000x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "Currency7500x",
                table: "ConsignmentDenomination");

            migrationBuilder.DropColumn(
                name: "Currency750x",
                table: "ConsignmentDenomination");
        }
    }
}
