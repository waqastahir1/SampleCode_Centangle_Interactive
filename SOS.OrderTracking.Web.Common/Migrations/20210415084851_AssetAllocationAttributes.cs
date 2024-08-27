﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AssetAllocationAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AssetAllocations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AssetAllocations");
        }
    }
}
