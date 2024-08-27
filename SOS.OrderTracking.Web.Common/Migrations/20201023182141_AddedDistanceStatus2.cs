using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedDistanceStatus2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "DistanceStatus",
                table: "Consignments",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceStatus",
                table: "Consignments");
        }
    }
}
