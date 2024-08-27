using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedAtmrDenomination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ATMRepairState",
                table: "ATMServices");

            migrationBuilder.AddColumn<int>(
                name: "Currency1000x",
                table: "ATMServices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency5000x",
                table: "ATMServices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Currency500x",
                table: "ATMServices",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency1000x",
                table: "ATMServices");

            migrationBuilder.DropColumn(
                name: "Currency5000x",
                table: "ATMServices");

            migrationBuilder.DropColumn(
                name: "Currency500x",
                table: "ATMServices");

            migrationBuilder.AddColumn<byte>(
                name: "ATMRepairState",
                table: "ATMServices",
                type: "tinyint",
                nullable: true);
        }
    }
}
