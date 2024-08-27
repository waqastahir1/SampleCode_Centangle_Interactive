using Microsoft.EntityFrameworkCore.Migrations;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class RefactoredNotificationsV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "NotificationSeq",
                startValue: 100L);

            migrationBuilder.AddColumn<string>(
                name: "ReceieverUserNam",
                table: "Notifications",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderUserName",
                table: "Notifications",
                maxLength: 450,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "NotificationSeq");

            migrationBuilder.DropColumn(
                name: "ReceieverUserNam",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderUserName",
                table: "Notifications");
        }
    }
}
