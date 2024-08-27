using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class EmployeeAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuardsAttendance");

            migrationBuilder.CreateTable(
                name: "EmployeeAttendance",
                columns: table => new
                {
                    RelationshipId = table.Column<int>(nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "date", nullable: false),
                    AttendanceState = table.Column<byte>(nullable: false),
                    CheckinTime = table.Column<DateTime>(nullable: false),
                    CheckoutTime = table.Column<DateTime>(nullable: true),
                    MarkedBy = table.Column<string>(maxLength: 450, nullable: false),
                    MarkedAt = table.Column<DateTime>(nullable: false),
                    Approvedby = table.Column<string>(maxLength: 450, nullable: true),
                    ApprovedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeAttendance", x => new { x.RelationshipId, x.AttendanceDate });
                    table.ForeignKey(
                        name: "FK_EmployeeAttendance_PartyRelationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "PartyRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeAttendance");

            migrationBuilder.CreateTable(
                name: "GuardsAttendance",
                columns: table => new
                {
                    RelationshipId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "date", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Approvedby = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AttendanceState = table.Column<byte>(type: "tinyint", nullable: false),
                    MarkedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MarkedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuardsAttendance", x => new { x.RelationshipId, x.AttendanceDate });
                    table.ForeignKey(
                        name: "FK_GuardsAttendance_PartyRelationships_RelationshipId",
                        column: x => x.RelationshipId,
                        principalTable: "PartyRelationships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }
    }
}
