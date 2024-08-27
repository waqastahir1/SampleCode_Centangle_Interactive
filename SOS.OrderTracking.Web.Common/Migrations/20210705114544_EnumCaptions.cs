using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class EnumCaptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CPCTransactions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CPCTransactionReasons",
                table: "CPCTransactions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CrPartRole_",
                table: "CPCTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrPartRole_",
                table: "CPCTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Particulars",
                table: "CPCTransactions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CPCTransactionReasons",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "CrPartRole_",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "DrPartRole_",
                table: "CPCTransactions");

            migrationBuilder.DropColumn(
                name: "Particulars",
                table: "CPCTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CPCTransactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);
        }
    }
}
