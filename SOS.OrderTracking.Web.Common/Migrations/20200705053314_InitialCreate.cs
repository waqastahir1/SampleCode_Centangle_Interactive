using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using System;

namespace SOS.OrderTracking.Web.Common.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "AssetsSeq",
                startValue: 100L);

            migrationBuilder.CreateSequence(
                name: "ATMROrdersSeq",
                startValue: 10000L);

            migrationBuilder.CreateSequence(
                name: "CITOrdersSeq",
                startValue: 10000L);

            migrationBuilder.CreateSequence(
                name: "CPCOrdersSeq",
                startValue: 10000L);

            migrationBuilder.CreateSequence(
                name: "PartiesSeq",
                startValue: 100L);

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PartyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    RegionId = table.Column<int>(nullable: true),
                    SubregionId = table.Column<int>(nullable: true),
                    StationId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AcquisitionDate = table.Column<DateTime>(nullable: false),
                    AssetType = table.Column<short>(nullable: false),
                    UpdateBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ATMServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ATMId = table.Column<int>(nullable: false),
                    CashBranchId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<int>(nullable: false),
                    TechnitianId = table.Column<int>(nullable: true),
                    CachierId = table.Column<int>(nullable: true),
                    ATMReplanishmentState = table.Column<byte>(nullable: true),
                    ATMRepairState = table.Column<byte>(nullable: true),
                    ATMRServiceType = table.Column<byte>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    ShipmentCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankCheckLists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false),
                    CheckListTypeId = table.Column<int>(nullable: false),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCheckLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargesTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargesTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckListTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckListTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConsignmentSealCodes",
                columns: table => new
                {
                    ConsignmentId = table.Column<int>(nullable: false),
                    SealCode = table.Column<string>(maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsignmentSealCodes", x => new { x.ConsignmentId, x.SealCode });
                });

            migrationBuilder.CreateTable(
                name: "DeliverStatuses",
                columns: table => new
                {
                    ConsignmentDeliveryId = table.Column<int>(nullable: false),
                    DeliveryStateType = table.Column<byte>(nullable: false),
                    ConsignmentId = table.Column<int>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    Tag = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverStatuses", x => new { x.ConsignmentDeliveryId, x.DeliveryStateType });
                });

            migrationBuilder.CreateTable(
                name: "DeliveryGeologs",
                columns: table => new
                {
                    TimeStamp = table.Column<long>(nullable: false),
                    DeliveryId = table.Column<int>(nullable: false),
                    Location = table.Column<Point>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryGeologs", x => new { x.TimeStamp, x.DeliveryId });
                });

            migrationBuilder.CreateTable(
                name: "DeviceCodes",
                columns: table => new
                {
                    UserCode = table.Column<string>(maxLength: 200, nullable: false),
                    DeviceCode = table.Column<string>(maxLength: 200, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceCodes", x => x.UserCode);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    Abbrevation = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 450, nullable: false),
                    Description = table.Column<string>(maxLength: 450, nullable: true),
                    Type = table.Column<byte>(nullable: false),
                    Geolocation = table.Column<Point>(nullable: true),
                    Status = table.Column<byte>(nullable: false),
                    LastSync = table.Column<DateTimeOffset>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    TableName = table.Column<string>(maxLength: 200, nullable: false),
                    Content = table.Column<string>(nullable: false),
                    ActionBy = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ReceiverId = table.Column<int>(nullable: false),
                    SenderId = table.Column<int>(nullable: false),
                    NotificationType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    HasSoundAlert = table.Column<bool>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ReceivedAt = table.Column<DateTime>(nullable: true),
                    ReadAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShortName = table.Column<string>(nullable: true),
                    FormalName = table.Column<string>(nullable: true),
                    Abbrevation = table.Column<string>(nullable: true),
                    PartyType = table.Column<int>(nullable: false),
                    PersonalContactNo = table.Column<string>(maxLength: 127, nullable: true),
                    OfficialContactNo = table.Column<string>(maxLength: 127, nullable: true),
                    PersonalEmail = table.Column<string>(maxLength: 127, nullable: true),
                    OfficialEmail = table.Column<string>(maxLength: 127, nullable: true),
                    Address = table.Column<string>(maxLength: 450, nullable: true),
                    RegionId = table.Column<int>(nullable: true),
                    SubregionId = table.Column<int>(nullable: true),
                    StationId = table.Column<int>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ExternalId = table.Column<int>(nullable: false),
                    SycStatus = table.Column<byte>(nullable: false),
                    ImageLink = table.Column<string>(maxLength: 450, nullable: true),
                    LastSync = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 200, nullable: true),
                    ClientId = table.Column<string>(maxLength: 200, nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Data = table.Column<string>(maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "ResourceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    RequestType = table.Column<byte>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    FromDate = table.Column<DateTime>(nullable: false),
                    ThruDate = table.Column<DateTime>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: false),
                    AllocationType = table.Column<byte>(nullable: false),
                    Remarks1 = table.Column<string>(nullable: true),
                    Remarks2 = table.Column<string>(nullable: true),
                    RequestedById = table.Column<string>(nullable: true),
                    RequestedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ATMServiceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ATMServiceId = table.Column<int>(nullable: false),
                    ATMServiceState = table.Column<byte>(nullable: false),
                    StateType = table.Column<byte>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ATMServiceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ATMServiceLogs_ATMServices_ATMServiceId",
                        column: x => x.ATMServiceId,
                        principalTable: "ATMServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LocationRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FromLocationId = table.Column<int>(nullable: false),
                    ToLocationId = table.Column<int>(nullable: false),
                    FromLocationType = table.Column<byte>(nullable: false),
                    ToLocationType = table.Column<byte>(nullable: false),
                    LastSync = table.Column<DateTimeOffset>(nullable: false),
                    Status = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationRelationships_Locations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationRelationships_Locations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    PartyId = table.Column<int>(nullable: false),
                    AllocatedFrom = table.Column<DateTime>(nullable: false),
                    AllocatedThru = table.Column<DateTime>(nullable: true),
                    AllocatedBy = table.Column<string>(nullable: true),
                    AllocatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetAllocations_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetAllocations_Parties_PartyId",
                        column: x => x.PartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Consignments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ShipmentCode = table.Column<string>(maxLength: 500, nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    FromPartyId = table.Column<int>(nullable: false),
                    ToPartyId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ServiceType = table.Column<byte>(nullable: false),
                    ShipmentType = table.Column<byte>(nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    CurrencySymbol = table.Column<byte>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    AmountPRK = table.Column<int>(nullable: false),
                    ConsignmentStatus = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consignments_Parties_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consignments_Parties_FromPartyId",
                        column: x => x.FromPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consignments_Parties_ToPartyId",
                        column: x => x.ToPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orgnizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Geolocation = table.Column<Point>(nullable: true),
                    OrganizationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orgnizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orgnizations_Parties_Id",
                        column: x => x.Id,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AttributeType = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PartyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyAttributes_Parties_Id",
                        column: x => x.Id,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyRelationships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FromPartyId = table.Column<int>(nullable: false),
                    ToPartyId = table.Column<int>(nullable: false),
                    FromPartyRole = table.Column<int>(nullable: false),
                    ToPartyRole = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    ThruDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartyRelationships_Parties_FromPartyId",
                        column: x => x.FromPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartyRelationships_Parties_ToPartyId",
                        column: x => x.ToPartyId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    NationalId = table.Column<string>(maxLength: 50, nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    DesignationId = table.Column<int>(nullable: true),
                    DesignationDesc = table.Column<string>(nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Parties_Id",
                        column: x => x.Id,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsignmentDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    PickupLocationId = table.Column<int>(nullable: false),
                    FromPartyId = table.Column<int>(nullable: false),
                    DestinationLocationId = table.Column<int>(nullable: false),
                    ToPartyId = table.Column<int>(nullable: false),
                    PlanedPickupTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ActualPickupTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    PlanedDropTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    ActualDropTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    CauseOfPickupTimeDifference = table.Column<int>(nullable: true),
                    CauseOfDropTimeDifference = table.Column<int>(nullable: true),
                    ConsignmentId = table.Column<int>(nullable: false),
                    CrewId = table.Column<int>(nullable: true),
                    PickupCode = table.Column<string>(maxLength: 450, nullable: true),
                    DropoffCode = table.Column<string>(maxLength: 450, nullable: true),
                    DeliveryStateType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsignmentDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsignmentDeliveries_Consignments_ConsignmentId",
                        column: x => x.ConsignmentId,
                        principalTable: "Consignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsignmentDeliveries_Parties_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Parties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsignmentDeliveries_Locations_DestinationLocationId",
                        column: x => x.DestinationLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsignmentDeliveries_Locations_PickupLocationId",
                        column: x => x.PickupLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Denominations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ConsignmentId = table.Column<int>(nullable: false),
                    CashBundleCount = table.Column<int>(nullable: false),
                    Currency1x = table.Column<int>(nullable: false),
                    Currency2x = table.Column<int>(nullable: false),
                    Currency5x = table.Column<int>(nullable: false),
                    Currency10x = table.Column<int>(nullable: false),
                    Currency20x = table.Column<int>(nullable: false),
                    Currency50x = table.Column<int>(nullable: false),
                    Currency100x = table.Column<int>(nullable: false),
                    Currency500x = table.Column<int>(nullable: false),
                    Currency1000x = table.Column<int>(nullable: false),
                    Currency5000x = table.Column<int>(nullable: false),
                    CashCountType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denominations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Denominations_Consignments_ConsignmentId",
                        column: x => x.ConsignmentId,
                        principalTable: "Consignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GuardsAttendance",
                columns: table => new
                {
                    RelationshipId = table.Column<int>(nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "date", nullable: false),
                    AttendanceState = table.Column<byte>(nullable: false),
                    MarkedBy = table.Column<string>(maxLength: 450, nullable: false),
                    MarkedAt = table.Column<DateTime>(nullable: false),
                    Approvedby = table.Column<string>(maxLength: 450, nullable: true),
                    ApprovedAt = table.Column<DateTime>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "DeliveryCharges",
                columns: table => new
                {
                    OrderFulfilmentId = table.Column<int>(nullable: false),
                    ChargeTypeId = table.Column<int>(nullable: false),
                    ConsignmentId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryCharges", x => new { x.OrderFulfilmentId, x.ChargeTypeId });
                    table.ForeignKey(
                        name: "FK_DeliveryCharges_ChargesTypes_ChargeTypeId",
                        column: x => x.ChargeTypeId,
                        principalTable: "ChargesTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryCharges_ConsignmentDeliveries_OrderFulfilmentId",
                        column: x => x.OrderFulfilmentId,
                        principalTable: "ConsignmentDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "CIT", null, "CIT", "CIT" },
                    { "BankGaurding", null, "BankGaurding", "BankGaurding" },
                    { "SOS-SubRegional-Admin", null, "SOS-SubRegional-Admin", "SOS-SUBREGIONAL-ADMIN" },
                    { "SOS-Regional-Admin", null, "SOS-Regional-Admin", "SOS-REGIONAL-ADMIN" },
                    { "BusinessOutlet", null, "BusinessOutlet", "BUSINESSOUTLET" },
                    { "Business", null, "Business", "BUSINESS" },
                    { "SOS-Admin", null, "SOS-Admin", "SOS-ADMIN" },
                    { "BankCPC", null, "BankCPC", "BANKCPC" },
                    { "BANK", null, "BANK", "BANK" },
                    { "ATMR", null, "ATMR", "ATMR" },
                    { "CPC", null, "CPC", "CPC" },
                    { "VAULT", null, "VAULT", "VAULT" },
                    { "BankBranch", null, "BankBranch", "BANKBRANCH" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PartyId", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c58e1a9d-1c28-46db-830a-7b3f0b9663f1", 0, "f15c8378-c937-4fc0-b590-c93b0f1aea4b", "asad@sos.com", true, true, null, "Asad Mehmood", "ASAD@SOS.COM", "ASAD@SOS.COM", 1, "AQAAAAEAACcQAAAAEDzPVLOfUXg6/0SLf64ejd+W/UCKgrZ87p7oo8MeJloKnlv64FMoQneGHrAZuoLOzA==", null, true, "7KAJF45HPZHW6SKSDBH4IAK5AFJZZALI", false, "asad@sos.com" });

            migrationBuilder.InsertData(
                table: "ChargesTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 8, "Waiting Charges" },
                    { 7, "Extra Charges" },
                    { 6, "Distance Charges" },
                    { 5, "Over Time Charges" },
                    { 2, "Surcharge" },
                    { 3, "Additional Charges" },
                    { 1, "Base Charges" },
                    { 9, "Toll Charges" },
                    { 4, "Seal Charges" }
                });

            migrationBuilder.InsertData(
                table: "Parties",
                columns: new[] { "Id", "Abbrevation", "Address", "ExternalId", "FormalName", "ImageLink", "LastSync", "OfficialContactNo", "OfficialEmail", "PartyType", "PersonalContactNo", "PersonalEmail", "RegionId", "ShortName", "StationId", "SubregionId", "SycStatus", "UpdatedAt" },
                values: new object[] { 1, null, "Islamabad", 0, "Security Organization Service", null, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, 32, "(051) 4576897", null, null, "SOS", null, null, (byte)0, null });

            migrationBuilder.InsertData(
                table: "Orgnizations",
                columns: new[] { "Id", "Geolocation", "OrganizationType" },
                values: new object[] { 1, null, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAllocations_AssetId",
                table: "AssetAllocations",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAllocations_PartyId",
                table: "AssetAllocations",
                column: "PartyId");

            migrationBuilder.CreateIndex(
                name: "IX_ATMServiceLogs_ATMServiceId",
                table: "ATMServiceLogs",
                column: "ATMServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_ConsignmentId",
                table: "ConsignmentDeliveries",
                column: "ConsignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_CrewId",
                table: "ConsignmentDeliveries",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_DestinationLocationId",
                table: "ConsignmentDeliveries",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsignmentDeliveries_PickupLocationId",
                table: "ConsignmentDeliveries",
                column: "PickupLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Consignments_CustomerId",
                table: "Consignments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Consignments_FromPartyId",
                table: "Consignments",
                column: "FromPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_Consignments_ToPartyId",
                table: "Consignments",
                column: "ToPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryCharges_ChargeTypeId",
                table: "DeliveryCharges",
                column: "ChargeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Denominations_ConsignmentId",
                table: "Denominations",
                column: "ConsignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCodes_DeviceCode",
                table: "DeviceCodes",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCodes_Expiration",
                table: "DeviceCodes",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRelationships_FromLocationId",
                table: "LocationRelationships",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRelationships_ToLocationId",
                table: "LocationRelationships",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationships_FromPartyId",
                table: "PartyRelationships",
                column: "FromPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationships_FromPartyRole",
                table: "PartyRelationships",
                column: "FromPartyRole");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationships_ToPartyId",
                table: "PartyRelationships",
                column: "ToPartyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartyRelationships_ToPartyRole",
                table: "PartyRelationships",
                column: "ToPartyRole");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_Expiration",
                table: "PersistedGrants",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AssetAllocations");

            migrationBuilder.DropTable(
                name: "ATMServiceLogs");

            migrationBuilder.DropTable(
                name: "BankCheckLists");

            migrationBuilder.DropTable(
                name: "CheckListTypes");

            migrationBuilder.DropTable(
                name: "ConsignmentSealCodes");

            migrationBuilder.DropTable(
                name: "DeliverStatuses");

            migrationBuilder.DropTable(
                name: "DeliveryCharges");

            migrationBuilder.DropTable(
                name: "DeliveryGeologs");

            migrationBuilder.DropTable(
                name: "Denominations");

            migrationBuilder.DropTable(
                name: "DeviceCodes");

            migrationBuilder.DropTable(
                name: "GuardsAttendance");

            migrationBuilder.DropTable(
                name: "LocationRelationships");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Orgnizations");

            migrationBuilder.DropTable(
                name: "PartyAttributes");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "ResourceRequests");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "ATMServices");

            migrationBuilder.DropTable(
                name: "ChargesTypes");

            migrationBuilder.DropTable(
                name: "ConsignmentDeliveries");

            migrationBuilder.DropTable(
                name: "PartyRelationships");

            migrationBuilder.DropTable(
                name: "Consignments");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropSequence(
                name: "AssetsSeq");

            migrationBuilder.DropSequence(
                name: "ATMROrdersSeq");

            migrationBuilder.DropSequence(
                name: "CITOrdersSeq");

            migrationBuilder.DropSequence(
                name: "CPCOrdersSeq");

            migrationBuilder.DropSequence(
                name: "PartiesSeq");
        }
    }
}
