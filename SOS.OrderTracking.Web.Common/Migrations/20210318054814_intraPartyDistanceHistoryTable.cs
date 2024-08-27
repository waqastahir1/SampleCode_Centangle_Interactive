using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class intraPartyDistanceHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "IntraPartyDistances");

            migrationBuilder.CreateSequence(
                name: "IntraPartyDistanceSeq",
                startValue: 100L);

            migrationBuilder.CreateTable(
                name: "IntraPartyDistancesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FromPartyId = table.Column<int>(type: "int", nullable: false),
                    ToPartyId = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    AverageTravelTime = table.Column<int>(type: "int", nullable: false),
                    DistanceStatus = table.Column<byte>(type: "tinyint", nullable: false),
                    DistanceSource = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntraPartyDistancesHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntraPartyDistancesHistory");

            migrationBuilder.DropSequence(
                name: "IntraPartyDistanceSeq");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "IntraPartyDistances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
