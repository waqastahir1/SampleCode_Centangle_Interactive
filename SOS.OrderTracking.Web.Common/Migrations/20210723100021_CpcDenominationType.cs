using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class CpcDenominationType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "DenominationTypeByCustomer",
                table: "CPCServices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "DenominationTypeBySos",
                table: "CPCServices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DenominationTypeByCustomer",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "DenominationTypeBySos",
                table: "CPCServices");
        }
    }
}
