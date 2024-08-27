using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedApprorvedVehicleCapacity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "DedicatedVehiclesSeq",
                startValue: 100L);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Parties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DedicatedVehiclesCapacities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    ToDate = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    VehicleCapacity = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DedicatedVehiclesCapacities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DedicatedVehiclesCapacities");

            migrationBuilder.DropSequence(
                name: "DedicatedVehiclesSeq");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Parties");
        }
    }
}
