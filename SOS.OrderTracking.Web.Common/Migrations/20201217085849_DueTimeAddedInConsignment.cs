using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class DueTimeAddedInConsignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledConsignments_Consignments_ConsignmentId",
                table: "ScheduledConsignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueTime",
                table: "Consignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledConsignments_Consignments_ConsignmentId",
                table: "ScheduledConsignments",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledConsignments_Consignments_ConsignmentId",
                table: "ScheduledConsignments");

            migrationBuilder.DropColumn(
                name: "DueTime",
                table: "Consignments");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledConsignments_Consignments_ConsignmentId",
                table: "ScheduledConsignments",
                column: "ConsignmentId",
                principalTable: "Consignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
