using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class ConsolidationV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orgnizations_IsCPCBranch",
                table: "Orgnizations",
                column: "IsCPCBranch");

            migrationBuilder.CreateIndex(
                name: "IX_Orgnizations_OrganizationType",
                table: "Orgnizations",
                column: "OrganizationType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orgnizations_IsCPCBranch",
                table: "Orgnizations");

            migrationBuilder.DropIndex(
                name: "IX_Orgnizations_OrganizationType",
                table: "Orgnizations");
        }
    }
}
