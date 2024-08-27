using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedLocationData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Abbrevation", "Code", "Description", "ExternalId", "Geolocation", "LastSync", "Name", "Status", "Type", "UpdatedAt" },
                values: new object[] { 1, "", "Dummy", "Dummy", 1, null, null, "Dummy", (byte)1, (byte)64, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
