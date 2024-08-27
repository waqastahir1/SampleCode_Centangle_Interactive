using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Models.CitShipment;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using ConsignmentState = SOS.OrderTracking.Web.Common.Data.Models.ConsignmentState;

namespace SOS.OrderTracking.Web.Common.Data
{
    public partial class AppDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public AppDbContext(
          DbContextOptions<AppDbContext> options,
          IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(29));
            Sequences = new SequenceService(this);
        }

        public virtual DbSet<Asset> Assets { get; set; }

        public virtual DbSet<AssetAllocation> AssetAllocations { get; set; }

        #region ATMR
        public virtual DbSet<ATMService> ATMServices { get; set; }

        public virtual DbSet<ATMServiceLog> ATMServiceLogs { get; set; }

        public virtual DbSet<ATMRSealCode> ATMRSealCodes { get; set; }

        #endregion

        #region Party

        public virtual DbSet<EmployeeAttendance> EmployeeAttendance { get; set; }

        public virtual DbSet<IntraPartyDistance> IntraPartyDistances { get; set; }

        public virtual DbSet<Organization> Orgnizations { get; set; }

        public virtual DbSet<Party> Parties { get; set; }

        public virtual DbSet<PartyAttribute> PartyAttributes { get; set; }

        public virtual DbSet<PartyLocation> PartyLocations { get; set; }

        public virtual DbSet<PartyRelationship> PartyRelationships { get; set; }

        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<GaurdingOrganization> GaurdingOrganizations { get; set; }

        #endregion

        #region Consignments

        public virtual DbSet<Consignment> Consignments { get; set; }
        public virtual DbSet<DeletedConsignment> DeletedConsignments { get; set; }

        public virtual DbSet<Denomination> Denominations { get; set; }

        public virtual DbSet<ShipmentSealCode> ShipmentSealCodes { get; set; }

        public virtual DbSet<ConsignmentDelivery> ConsignmentDeliveries { get; set; }

        public virtual DbSet<ShipmentChargeType> ShipmentChargeType { get; set; }

        public virtual DbSet<ConsignmentState> ConsignmentStates { get; set; }
        public virtual DbSet<ShipmentAttachment> ShipmentAttachments { get; set; }

        public virtual DbSet<ShipmentCharge> ShipmentCharges { get; set; }
        public virtual DbSet<ScheduledConsignment> ScheduledConsignments { get; set; }

        public virtual DbSet<VaultedShipment> VaultedShipments { get; set; }
        public virtual DbSet<VaultedSeal> VaultedSeals { get; set; }

        #endregion

        #region Consignments

        public virtual DbSet<CPCService> CPCServices { get; set; }

        public virtual DbSet<CPCTransaction> CPCTransactions { get; set; }
        #endregion

        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<LocationRelationship> LocationRelationships { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Log> Logs { get; set; }

        public virtual DbSet<DeliveryGeolog> DeliveryGeologs { get; set; }

        public virtual DbSet<ResourceRequest> ResourceRequests { get; set; }

        public virtual DbSet<CheckListType> CheckListTypes { get; set; }

        public virtual DbSet<BankCheckList> BankCheckLists { get; set; }
        public virtual DbSet<Complaint> Complaints { get; set; }
        public virtual DbSet<ComplaintCategory> ComplaintCategories { get; set; }
        public virtual DbSet<Models.ComplaintStatus> ComplaintStatuses { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<DedicatedVehiclesCapacity> DedicatedVehiclesCapacities { get; set; }
        public virtual DbSet<IntraPartyDistanceHistory> IntraPartyDistancesHistory { get; set; }
        public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }

        public DbSet<ShipmentDeliveryApiLog> ShipmentDeliveryApiLogs { get; set; }
        public virtual DbSet<AllocatedBranch> AllocatedBranches { get; set; }
        public virtual DbSet<MonthlyShipmentRange> MonthlyShipmentRanges { get; set; }
        public virtual DbSet<AllocatedRange> AllocatedRanges { get; set; }
        public DbSet<CancelledShipment> CancelledShipments { get; set; }
        public DbSet<MissingShipment> MissingShipments { get; set; }
        public DbSet<GbmsShipmentsScanStatus> GbmsShipmentsScanStatuses { get; set; }

        public SequenceService Sequences { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AssetAllocation>(entity =>
            {
                entity.HasOne(d => d.Asset)
                    .WithMany(p => p.AssetAllocations)
                    .HasForeignKey(d => d.AssetId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.AssetAllocations)
                    .HasForeignKey(d => d.PartyId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });


            modelBuilder.Entity<LocationRelationship>(entity =>
            {
                entity.HasIndex(e => e.FromLocationId);

                entity.HasIndex(e => e.ToLocationId);

                entity.HasOne(d => d.FromLocation)
                    .WithMany(p => p.LocationRelationshipsFromLocation)
                    .HasForeignKey(d => d.FromLocationId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasOne(d => d.ToLocation)
                    .WithMany(p => p.LocationRelationshipsToLocation)
                    .HasForeignKey(d => d.ToLocationId)
                    .OnDelete(DeleteBehavior.ClientCascade);
            });
            //modelBuilder.Entity<ScheduledConsignment>(entity =>
            //{
            //    entity.HasIndex(e => e.ConsignmentId);

            //    entity.HasOne(d => d.FkConsignment)
            //    .WithMany(p => p.ScheduledConsignments)
            //    .HasForeignKey(d => d.ConsignmentId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //});

            modelBuilder.Entity<ComplaintCategory>().HasKey(x => new { x.CategoryId, x.ComplaintId });

            modelBuilder.Entity<ShipmentDeliveryApiLog>().HasKey(x => new
            {
                x.ShipmentDeliveryId,
                x.ShipmentState,
                x.Status
            });


            modelBuilder.Entity<ShipmentCharge>(entity =>
            {
                entity.HasKey(e => new { e.ConsignmentId, e.ChargeTypeId });

                entity.HasIndex(e => e.ChargeTypeId);

                entity.HasOne(d => d.ChargeType)
                    .WithMany(p => p.DeliveryCharges)
                    .HasForeignKey(d => d.ChargeTypeId);

                entity.HasOne(d => d.Consignment)
                    .WithMany(p => p.DeliveryCharges)
                    .HasForeignKey(d => d.ConsignmentId);
            });

            modelBuilder.Entity<ShipmentSealCode>(entity =>
            {
                entity.HasKey(e => new { e.ConsignmentId, e.SealCode });

            });

            modelBuilder.Entity<ATMRSealCode>(entity =>
            {
                entity.HasKey(e => new { e.AtmrServiceId, e.SealCode });

            });

            modelBuilder.Entity<ConsignmentDelivery>(entity =>
            {
                entity.HasIndex(e => e.CrewId);

                entity.HasIndex(e => e.DestinationLocationId);
                entity.HasIndex(e => e.FromPartyId);

                entity.HasIndex(e => e.ConsignmentId);

                entity.HasIndex(e => e.PickupLocationId);
                entity.HasIndex(e => e.ToPartyId);

                entity.HasOne(d => d.Crew)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.CrewId);

                entity.HasOne(d => d.DestinationLocation)
                 .WithMany(p => p.OrderFulfilmentsDestinationLocation)
                 .HasForeignKey(d => d.DestinationLocationId)
                 .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(d => d.FromParty)
                    .WithMany(p => p.FromPartyDeliveries)
                    .HasForeignKey(d => d.FromPartyId)
                    .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(d => d.Consignment)
                    .WithMany(p => p.ConsignmentDeliveries)
                    .HasForeignKey(d => d.ConsignmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.PickupLocation)
                              .WithMany(p => p.OrderFulfilmentsPickupLocation)
                              .HasForeignKey(d => d.PickupLocationId)
                              .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(d => d.ToParty)
                    .WithMany(p => p.ToPartyDeliveries)
                    .HasForeignKey(d => d.ToPartyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Parent)
                     .WithMany(p => p.Childern)
                     .HasForeignKey(d => d.ParentId)
                     .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasOne(d => d.Party)
                    .WithOne(p => p.Orgnization)
                    .HasForeignKey<Organization>(d => d.Id);

                entity.HasIndex(x => x.IsCPCBranch);
                entity.HasIndex(x => x.OrganizationType);
            });

            modelBuilder.Entity<PartyAttribute>(entity =>
            {
                entity.HasOne(d => d.Party)
                    .WithOne(p => p.PartyAttributes)
                    .HasForeignKey<PartyAttribute>(d => d.Id);
            });

            modelBuilder.Entity<PartyRelationship>(entity =>
            {
                entity.HasIndex(e => e.FromPartyId);

                entity.HasIndex(e => e.FromPartyRole);

                entity.HasIndex(e => e.ToPartyId);

                entity.HasIndex(e => e.ToPartyRole);

                entity.HasOne(d => d.FromParty)
                    .WithMany(p => p.PartyRelationshipsFromParty)
                    .HasForeignKey(d => d.FromPartyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ToParty)
                    .WithMany(p => p.PartyRelationshipsToParty)
                    .HasForeignKey(d => d.ToPartyId)
                    .OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<PartyLocation>(entity =>
            {
                entity.HasKey(d => new
                {
                    d.PartyId,
                    d.TimeStamp
                });
            });


            modelBuilder.Entity<IntraPartyDistance>(entity =>
            {
                entity.HasKey(d => new
                {
                    d.FromPartyId,
                    d.ToPartyId
                });
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("People");

                entity.HasOne(d => d.Origin)
                    .WithOne(p => p.People)
                    .HasForeignKey<Person>(d => d.Id);
            });

            modelBuilder.Entity<Denomination>(entity =>
            {
                entity.HasIndex(e => e.ConsignmentId);


                entity.HasOne(d => d.FkWorkOrder)
                    .WithMany(p => p.Denominations)
                    .HasForeignKey(d => d.ConsignmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Consignment>(entity =>
            {
                entity.HasIndex(e => e.FromPartyId);

                entity.HasIndex(e => e.ToPartyId);

                entity.HasOne(d => d.MainCustomer)
               .WithMany(p => p.MainCustomerOrders)
               .HasForeignKey(d => d.MainCustomerId)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Customer)
               .WithMany(p => p.CustomerOrders)
               .HasForeignKey(d => d.FromPartyId)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.BillBranch)
                  .WithMany(p => p.BillBranchOrders)
                  .HasForeignKey(d => d.BillBranchId)
                  .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.FromParty)
                    .WithMany(p => p.FromPartyOrders)
                    .HasForeignKey(d => d.FromPartyId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ToParty)
                    .WithMany(p => p.ToPartyOrders)
                    .HasForeignKey(d => d.ToPartyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DeliveryGeolog>(entity =>
            {
                entity.HasKey(x => new { x.TimeStamp, x.DeliveryId });
            });

            modelBuilder.Entity<ConsignmentState>(entity =>
            {
                entity.HasKey(x => new { x.ConsignmentId, x.DeliveryId, x.ConsignmentStateType });
            });


            modelBuilder.Entity<ATMServiceLog>(entity =>
            {
                entity.HasOne(d => d.ATMService)
               .WithMany(p => p.ATMServiceLogs)
               .HasForeignKey(d => d.ATMServiceId)
               .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.HasSequence(SequenceService.CITOrdersSeq)
                .StartsAt(10000);

            modelBuilder.HasSequence(SequenceService.CITDeliveriesSeq)
        .StartsAt(10000);

            modelBuilder.HasSequence(SequenceService.CPCOrdersSeq)
                .StartsAt(10000);

            modelBuilder.HasSequence(SequenceService.CPCTransactionsSeq)
             .StartsAt(10000);

            modelBuilder.HasSequence(SequenceService.ATMROrdersSeq)
                .StartsAt(10000);

            modelBuilder.HasSequence(SequenceService.PartiesSeq)
                .StartsAt(100);

            modelBuilder.HasSequence(SequenceService.AssetsSeq)
                .StartsAt(100);


            modelBuilder.HasSequence(SequenceService.NotificationSeq)
                .StartsAt(100);

            modelBuilder.HasSequence(SequenceService.DedicatedVehiclesSeq)
              .StartsAt(100);

            modelBuilder.HasSequence(SequenceService.DedicatedVehiclesSeq)
            .StartsAt(100);

            modelBuilder.HasSequence(SequenceService.IntraPartyDistanceSeq)
                .StartsAt(100);

            modelBuilder.Entity<EmployeeAttendance>(entity =>
            {

                entity.ToTable("EmployeeAttendance");

                entity.HasOne(g => g.AllocationRelationship)
                    .WithMany(p => p.EmployeeAttendances)
                    .HasForeignKey(g => g.RelationshipId)
                    .OnDelete(DeleteBehavior.ClientCascade);

                entity.HasKey(g => new { g.RelationshipId, g.AttendanceDate });

            });




            SeedData(modelBuilder);
        }


        public override int SaveChanges()
        {
            if (DatabaseActionController.ReadOnly)
                throw new InvalidOperationException("An unhandled exception of type 'System.Data.SqlClient.SqlException' occurred in System.Data.dll");
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            if (DatabaseActionController.ReadOnly)
                throw new InvalidOperationException("An unhandled exception of type 'System.Data.SqlClient.SqlException' occurred in System.Data.dll");
            return base.SaveChangesAsync();
        }

        public void SeedData(ModelBuilder modelBuilder)
        {
            //A.B. Keep decent track of IDs inserted for Party (Person, Organization)
            //post increment this before assigning to any party
            int partyId = 0;

            modelBuilder.Entity<Party>().HasData(
             new Party
             {
                 Id = partyId,
                 PartyType = PartyType.Organization,
                 Address = "Nill",
                 PersonalContactNo = "Nill",
                 FormalName = "Nill",
                 ShortName = "Nill"
             });
            modelBuilder.Entity<Organization>().HasData(
                new Organization
                {
                    Id = partyId,
                    OrganizationType = OrganizationType.Unknown,
                    Geolocation = null
                });

            #region SOS-1
            modelBuilder.Entity<Party>().HasData(
                new Party
                {
                    Id = ++partyId,
                    PartyType = PartyType.Organization,
                    Address = "Islamabad",
                    PersonalContactNo = "(051) 4576897",
                    FormalName = "Security Organization Service",
                    ShortName = "SOS"
                });
            modelBuilder.Entity<Organization>().HasData(
                new Organization
                {
                    Id = partyId,
                    OrganizationType = OrganizationType.PrimaryOrganization,
                    Geolocation = null
                });

            #endregion

            #region Users 
            modelBuilder.Entity<ApplicationUser>().HasData(
            new ApplicationUser()
            {
                Id = "c58e1a9d-1c28-46db-830a-7b3f0b9663f1",
                UserName = "asad@sos.com",
                NormalizedUserName = "ASAD@SOS.COM",
                Email = "asad@sos.com",
                NormalizedEmail = "ASAD@SOS.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAEDzPVLOfUXg6/0SLf64ejd+W/UCKgrZ87p7oo8MeJloKnlv64FMoQneGHrAZuoLOzA==",
                SecurityStamp = "7KAJF45HPZHW6SKSDBH4IAK5AFJZZALI",
                ConcurrencyStamp = "f15c8378-c937-4fc0-b590-c93b0f1aea4b",
                PhoneNumber = null,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnd = null,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                Name = "Asad Mehmood",
                PartyId = 1
            });
            #endregion

            #region Roles 
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "CIT", Name = "CIT", NormalizedName = "CIT", ConcurrencyStamp = null },
                new IdentityRole() { Id = "VAULT", Name = "VAULT", NormalizedName = "VAULT", ConcurrencyStamp = null },
                new IdentityRole() { Id = "CPC", Name = "CPC", NormalizedName = "CPC", ConcurrencyStamp = null },
                new IdentityRole() { Id = "ATMR", Name = "ATMR", NormalizedName = "ATMR", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BANK", Name = "BANK", NormalizedName = "BANK", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BankCPC", Name = "BankCPC", NormalizedName = "BANKCPC", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BankBranch", Name = "BankBranch", NormalizedName = "BANKBRANCH", ConcurrencyStamp = null },
                new IdentityRole() { Id = "Business", Name = "Business", NormalizedName = "BUSINESS", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BusinessOutlet", Name = "BusinessOutlet", NormalizedName = "BUSINESSOUTLET", ConcurrencyStamp = null },
                new IdentityRole() { Id = "SOS-Regional-Admin", Name = "SOS-Regional-Admin", NormalizedName = "SOS-REGIONAL-ADMIN", ConcurrencyStamp = null },
                new IdentityRole() { Id = "SOS-SubRegional-Admin", Name = "SOS-SubRegional-Admin", NormalizedName = "SOS-SUBREGIONAL-ADMIN", ConcurrencyStamp = null },
                new IdentityRole() { Id = "SOS-Admin", Name = "SOS-Admin", NormalizedName = "SOS-ADMIN", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BankGaurding", Name = "BankGaurding", NormalizedName = "BankGaurding", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BankBranchManager", Name = "BankBranchManager", NormalizedName = "BankBranchManager", ConcurrencyStamp = null },
                new IdentityRole() { Id = "BankCPCManager", Name = "BankCPCManager", NormalizedName = "BankCPCManager", ConcurrencyStamp = null },
                new IdentityRole() { Id = "SOS-Headoffice-Billing", Name = "SOS-Headoffice-Billing", NormalizedName = "SOS-Headoffice-Billing".ToUpper(), ConcurrencyStamp = null },
                new IdentityRole() { Id = "Edit-Distance-Draft", Name = "Edit-Distance-Draft", NormalizedName = "Edit-Distance-Draft".ToUpper(), ConcurrencyStamp = null },
                new IdentityRole() { Id = "Edit-Distance-Approve", Name = "Edit-Distance-Approve", NormalizedName = "Edit-Distance-Approve".ToUpper(), ConcurrencyStamp = null },
                new IdentityRole() { Id = "Super-Admin", Name = "Super-Admin", NormalizedName = "Super-Admin".ToUpper(), ConcurrencyStamp = null });
            #endregion

            #region UserRoles 

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "SOS-Admin",
                UserId = "c58e1a9d-1c28-46db-830a-7b3f0b9663f1"
            });
            #endregion

            modelBuilder.Entity<Location>()
                .HasData(new Location()
                {
                    Id = 1,
                    Code = "Dummy",
                    Description = "Dummy",
                    ExternalId = 1,
                    Abbrevation = "",
                    Name = "Dummy",
                    Status = 1,
                    Type = LocationType.Address,
                    UpdatedAt = new DateTime(2020, 1, 1)
                });

            #region Delivery Charges Type

            modelBuilder.Entity<ShipmentChargeType>().HasData(
                //new ShipmentChargeType() { Id = 1, Name = "Base Charges" },
                //new ShipmentChargeType() { Id = 2, Name = "Surcharge" },
                //new ShipmentChargeType() { Id = 3, Name = "Additional Charges" },
                //new ShipmentChargeType() { Id = 4, Name = "Seal Charges" },
                //new ShipmentChargeType() { Id = 5, Name = "Over Time Charges" },
                //new ShipmentChargeType() { Id = 6, Name = "Distance Charges" },
                //new ShipmentChargeType() { Id = 7, Name = "Extra Charges" },
                new ShipmentChargeType() { Id = 1, Name = "Waiting Charges" },
                new ShipmentChargeType() { Id = 2, Name = "Toll Charges" });

            #endregion

            #region Categories
            modelBuilder.Entity<Category>()
                .HasData(new Category
                {
                    Id = 1,
                    Name = "Bad Behaviour",
                    IsActive = true,
                    CreatedAt = new DateTime(2020, 1, 1),
                    CreatedBy = "asad@sos.com"
                });
            modelBuilder.Entity<Category>()
              .HasData(new Category
              {
                  Id = 2,
                  Name = "Bad Quality",
                  IsActive = true,
                  CreatedAt = new DateTime(2020, 1, 1),
                  CreatedBy = "asad@sos.com"
              });
            modelBuilder.Entity<Category>()
              .HasData(new Category
              {
                  Id = 3,
                  Name = "Shipment Delayed",
                  IsActive = true,
                  CreatedAt = new DateTime(2020, 1, 1),
                  CreatedBy = "asad@sos.com",
              });
            #endregion

        }


    }
}

