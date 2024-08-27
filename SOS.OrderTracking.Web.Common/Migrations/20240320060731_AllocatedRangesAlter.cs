using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AllocatedRangesAlter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCrew",
                table: "AllocatedRanges",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "AllocatedRanges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdated",
                table: "AllocatedRanges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "AllocatedRanges",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCrew",
                table: "AllocatedRanges");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "AllocatedRanges");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "AllocatedRanges");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "AllocatedRanges");
        }
    }
}
