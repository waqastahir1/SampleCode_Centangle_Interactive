using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RefactoredNotificationsV3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceieverUserNam",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserName",
                table: "Notifications",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Notifications",
                maxLength: 450,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverUserName",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ReceieverUserNam",
                table: "Notifications",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }
    }
}
