using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class UpdatedPartiesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Parties",
                newName: "UpdatedAtErp");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Parties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Parties");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtErp",
                table: "Parties",
                newName: "UpdatedAt");
        }
    }
}
