using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class ReverseAllocationRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "AllocatedRanges",
                newName: "LastUpdatedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedAt",
                table: "AllocatedRanges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 03, 18, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AddedBy",
                table: "AllocatedRanges",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedAt",
                table: "AllocatedRanges");

            migrationBuilder.DropColumn(
                name: "AddedBy",
                table: "AllocatedRanges");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedBy",
                table: "AllocatedRanges",
                newName: "LastUpdated");
        }
    }
}
