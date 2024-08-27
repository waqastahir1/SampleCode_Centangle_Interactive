using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class CpcDenomination : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer1000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer100x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer10x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer20x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer5000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyByCustomer50x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos1000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos100x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos10x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos20x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos5000x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos500x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyBySos50x",
                table: "CPCServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "CPCServices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedAt",
                table: "CPCServices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedBy",
                table: "CPCServices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "ReceivingMode",
                table: "CPCServices",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer1000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer100x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer10x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer20x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer5000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyByCustomer50x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos1000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos100x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos10x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos20x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos5000x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos500x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "CurrencyBySos50x",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "ReceivedAt",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "CPCServices");

            migrationBuilder.DropColumn(
                name: "ReceivingMode",
                table: "CPCServices");
        }
    }
}
