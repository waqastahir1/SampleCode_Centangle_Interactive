using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class NewDropoffInConsignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonData",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsToChangedPartyVerified",
                table: "Consignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ToChangedPartyId",
                table: "Consignments",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsToChangedPartyVerified",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "ToChangedPartyId",
                table: "Consignments");

            migrationBuilder.AddColumn<string>(
                name: "JsonData",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
