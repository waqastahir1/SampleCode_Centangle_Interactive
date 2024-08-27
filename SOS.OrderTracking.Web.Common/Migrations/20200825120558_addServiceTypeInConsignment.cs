using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class addServiceTypeInConsignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPRK",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "Valuables",
                table: "Consignments");

            migrationBuilder.AddColumn<int>(
                name: "AmountPKR",
                table: "Consignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "Consignments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Valueables",
                table: "Consignments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPKR",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "Valueables",
                table: "Consignments");

            migrationBuilder.AddColumn<int>(
                name: "AmountPRK",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Valuables",
                table: "Consignments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
