using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class AddedDistanceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateAt",
                table: "Parties",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "GeolocationUpdateAt",
                table: "Orgnizations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "GeolocationVersion",
                table: "Orgnizations",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<byte>(
                name: "DistanceStatus",
                table: "IntraPartyDistances",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "IntraPartyDistances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "IntraPartyDistances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Parties",
                keyColumn: "Id",
                keyValue: 0,
                column: "UpdateAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Parties",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdateAt",
                value: null);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeolocationUpdateAt",
                table: "Orgnizations");

            migrationBuilder.DropColumn(
                name: "GeolocationVersion",
                table: "Orgnizations");

            migrationBuilder.DropColumn(
                name: "DistanceStatus",
                table: "IntraPartyDistances");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "IntraPartyDistances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "IntraPartyDistances");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateAt",
                table: "Parties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Parties",
                keyColumn: "Id",
                keyValue: 0,
                column: "UpdateAt",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Parties",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdateAt",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
