using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedPartyLocationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntraPartyDistances",
                columns: table => new
                {
                    FromPartyId = table.Column<int>(nullable: false),
                    ToPartyId = table.Column<int>(nullable: false),
                    Distance = table.Column<double>(nullable: false),
                    AverageTravelTime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntraPartyDistances", x => new { x.FromPartyId, x.ToPartyId });
                });

            migrationBuilder.CreateTable(
                name: "PartyLocations",
                columns: table => new
                {
                    PartyId = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Geolocation = table.Column<Point>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatdBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyLocations", x => new { x.PartyId, x.TimeStamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntraPartyDistances");

            migrationBuilder.DropTable(
                name: "PartyLocations");
        }
    }
}
