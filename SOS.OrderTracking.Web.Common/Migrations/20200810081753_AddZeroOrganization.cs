using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddZeroOrganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Parties",
                columns: new[] { "Id", "Abbrevation", "Address", "ExternalId", "FormalName", "ImageLink", "LastSync", "OfficialContactNo", "OfficialEmail", "ParentCode", "PartyType", "PersonalContactNo", "PersonalEmail", "RegionId", "ShortName", "StationId", "SubregionId", "SycStatus", "UpdatedAt" },
                values: new object[] { 0, null, "Nill", 0, "Nill", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, null, 32, "Nill", null, null, "Nill", null, null, (byte)0, null });

            migrationBuilder.InsertData(
                table: "Orgnizations",
                columns: new[] { "Id", "ExternalBranchType", "ExternalCustomerType", "Geolocation", "IsCPCBranch", "OrganizationType" },
                values: new object[] { 0, null, null, null, false, 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orgnizations",
                keyColumn: "Id",
                keyValue: 0);

            migrationBuilder.DeleteData(
                table: "Parties",
                keyColumn: "Id",
                keyValue: 0);
        }
    }
}
