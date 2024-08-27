using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RangeAllocationToRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RangeAllocatedToRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RangeStart = table.Column<int>(type: "int", nullable: false),
                    RangeEnd = table.Column<int>(type: "int", nullable: false),
                    MonthlyRangeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangeAllocatedToRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RangeAllocatedToRegions_MonthlyShipmentRanges_MonthlyRangeId",
                        column: x => x.MonthlyRangeId,
                        principalTable: "MonthlyShipmentRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RangeAllocatedToRegions_MonthlyRangeId",
                table: "RangeAllocatedToRegions",
                column: "MonthlyRangeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RangeAllocatedToRegions");
        }
    }
}
