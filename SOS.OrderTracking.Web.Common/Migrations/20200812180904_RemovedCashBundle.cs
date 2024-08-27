using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RemovedCashBundle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashBundleCount",
                table: "ConsignmentDenomination");

            //    migrationBuilder.AlterColumn<string>(
            //        name: "Name",
            //        table: "AspNetUserTokens",
            //        maxLength: 128,
            //        nullable: false,
            //        oldClrType: typeof(string),
            //        oldType: "nvarchar(450)");

            //    migrationBuilder.AlterColumn<string>(
            //        name: "LoginProvider",
            //        table: "AspNetUserTokens",
            //        maxLength: 128,
            //        nullable: false,
            //        oldClrType: typeof(string),
            //        oldType: "nvarchar(450)");

            //    migrationBuilder.AlterColumn<string>(
            //        name: "ProviderKey",
            //        table: "AspNetUserLogins",
            //        maxLength: 128,
            //        nullable: false,
            //        oldClrType: typeof(string),
            //        oldType: "nvarchar(450)");

            //    migrationBuilder.AlterColumn<string>(
            //        name: "LoginProvider",
            //        table: "AspNetUserLogins",
            //        maxLength: 128,
            //        nullable: false,
            //        oldClrType: typeof(string),
            //        oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CashBundleCount",
                table: "ConsignmentDenomination",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);
        }
    }
}
