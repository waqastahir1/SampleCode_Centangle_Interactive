using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedCustomerDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalBranchType",
                table: "Orgnizations",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalCustomerType",
                table: "Orgnizations",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCPCBranch",
                table: "Orgnizations",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalBranchType",
                table: "Orgnizations");

            migrationBuilder.DropColumn(
                name: "ExternalCustomerType",
                table: "Orgnizations");

            migrationBuilder.DropColumn(
                name: "IsCPCBranch",
                table: "Orgnizations");
        }
    }
}
