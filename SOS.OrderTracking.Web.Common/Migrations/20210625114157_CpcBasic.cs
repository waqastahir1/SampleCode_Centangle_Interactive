using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class CpcBasic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "CPCTransactionsSeq",
                startValue: 1L);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "CPCServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ShipmentCode = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    MainCustomerId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FromPartyId = table.Column<int>(type: "int", nullable: false),
                    ToPartyId = table.Column<int>(type: "int", nullable: false),
                    BillBranchId = table.Column<int>(type: "int", nullable: false),
                    CollectionRegionId = table.Column<int>(type: "int", nullable: false),
                    CollectionSubRegionId = table.Column<int>(type: "int", nullable: false),
                    CollectionStationId = table.Column<int>(type: "int", nullable: false),
                    DeliveryRegionId = table.Column<int>(type: "int", nullable: false),
                    DeliverySubRegionId = table.Column<int>(type: "int", nullable: false),
                    DeliveryStationId = table.Column<int>(type: "int", nullable: false),
                    BillingRegionId = table.Column<int>(type: "int", nullable: false),
                    BillingSubRegionId = table.Column<int>(type: "int", nullable: false),
                    BillingStationId = table.Column<int>(type: "int", nullable: false),
                    ApprovalState = table.Column<byte>(type: "tinyint", nullable: false),
                    ConsignmentStatus = table.Column<short>(type: "smallint", nullable: false),
                    CpcServiceCategory = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CurrencySymbol = table.Column<byte>(type: "tinyint", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    AmountPKR = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsignmentStateType = table.Column<byte>(type: "tinyint", nullable: false),
                    PostingMessage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPCServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CPCTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CPCServiceId = table.Column<int>(type: "int", nullable: false),
                    FromPartyId = table.Column<int>(type: "int", nullable: false),
                    ToPartyId = table.Column<int>(type: "int", nullable: false),
                    CPCTransactionType = table.Column<short>(type: "smallint", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    TransactionFactor = table.Column<short>(type: "smallint", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Currency10x = table.Column<int>(type: "int", nullable: false),
                    Currency20x = table.Column<int>(type: "int", nullable: false),
                    Currency50x = table.Column<int>(type: "int", nullable: false),
                    Currency100x = table.Column<int>(type: "int", nullable: false),
                    Currency500x = table.Column<int>(type: "int", nullable: false),
                    Currency1000x = table.Column<int>(type: "int", nullable: false),
                    Currency5000x = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectsCustomerBalance = table.Column<bool>(type: "bit", nullable: false),
                    EffectsProcessedBalance = table.Column<bool>(type: "bit", nullable: false),
                    TotalBalance = table.Column<int>(type: "int", nullable: false),
                    ProcessedBalance = table.Column<int>(type: "int", nullable: false),
                    UnprocessedBalance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CPCTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CPCServices");

            migrationBuilder.DropTable(
                name: "CPCTransactions");

            migrationBuilder.DropSequence(
                name: "CPCTransactionsSeq");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
