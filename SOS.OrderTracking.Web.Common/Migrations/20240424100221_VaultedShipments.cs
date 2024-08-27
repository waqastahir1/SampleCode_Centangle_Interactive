using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class VaultedShipments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VaultedShipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentId = table.Column<int>(type: "int", nullable: false),
                    VaultId = table.Column<int>(type: "int", nullable: false),
                    CrewInId = table.Column<int>(type: "int", nullable: false),
                    CrewOutId = table.Column<int>(type: "int", nullable: false),
                    AmountIn = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountOut = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TimeIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeOut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiredTimeOut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VaultedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultedShipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultedSeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SealCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VaultedShipmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultedSeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultedSeals_VaultedShipments_VaultedShipmentId",
                        column: x => x.VaultedShipmentId,
                        principalTable: "VaultedShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaultedSeals_VaultedShipmentId",
                table: "VaultedSeals",
                column: "VaultedShipmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaultedSeals");

            migrationBuilder.DropTable(
                name: "VaultedShipments");
        }
    }
}
