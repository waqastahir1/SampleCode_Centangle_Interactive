using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class SOSOrderTrackingContext : DbContext
    {
        public SOSOrderTrackingContext()
        {
        }

        public SOSOrderTrackingContext(DbContextOptions<SOSOrderTrackingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChargesType> ChargesTypes { get; set; }
        public virtual DbSet<LocationRelationship> LocationRelationships { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<OrderFulfilmentCharge> OrderFulfilmentCharges { get; set; }
        public virtual DbSet<OrderFulfilment> OrderFulfilments { get; set; }
        public virtual DbSet<Orgnization> Orgnizations { get; set; }
        public virtual DbSet<Party> Parties { get; set; }
        public virtual DbSet<PartyAttribute> PartyAttributes { get; set; }
        public virtual DbSet<PartyLocation> PartyLocations { get; set; }
        public virtual DbSet<PartyRelationship> PartyRelationships { get; set; }
        public virtual DbSet<PartyRole> PartyRoles { get; set; }
        public virtual DbSet<People> Peoples { get; set; }
        public virtual DbSet<RoleType> RoleTypes { get; set; }
        public virtual DbSet<WorkOrderItem> WorkOrderItems { get; set; }
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LAPTOP-DMUAS1ET\\MSSQLSERVER2019;Database=SOSOrderTracking;User Id=sa;Password=123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChargesType>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<LocationRelationship>(entity =>
            {
                entity.HasOne(d => d.FromLocation)
                    .WithMany(p => p.LocationRelationshipsFromLocation)
                    .HasForeignKey(d => d.FromLocationId)
                    .HasConstraintName("FK_LocationRelationships_FromLocations");

                entity.HasOne(d => d.ToLocation)
                    .WithMany(p => p.LocationRelationshipsToLocation)
                    .HasForeignKey(d => d.ToLocationId)
                    .HasConstraintName("FK_LocationRelationships_ToLocations");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.ActionBy).HasMaxLength(100);

                entity.Property(e => e.TableName).HasMaxLength(200);

                entity.Property(e => e.TimeStamp).HasColumnType("datetime");
            });

            modelBuilder.Entity<OrderFulfilmentCharge>(entity =>
            {
                entity.HasKey(e => new { e.OrderFulfilmentId, e.ChargesTypeId });

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.ChargesType)
                    .WithMany(p => p.OrderFulfilmentCharges)
                    .HasForeignKey(d => d.ChargesTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderFulfilmentCharges_ChargesTypes");

                entity.HasOne(d => d.OrderFulfilment)
                    .WithMany(p => p.OrderFulfilmentCharges)
                    .HasForeignKey(d => d.OrderFulfilmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderFulfilmentCharges_OrderFulfilments");
            });

            modelBuilder.Entity<OrderFulfilment>(entity =>
            {
                entity.Property(e => e.ActualDropTime).HasColumnType("datetime");

                entity.Property(e => e.ActualPickupTime).HasColumnType("datetime");

                entity.Property(e => e.FkWorkOrderId).HasColumnName("FK_WorkOrder_Id");

                entity.Property(e => e.PlanedDropTime).HasColumnType("datetime");

                entity.Property(e => e.PlanedPickupTime).HasColumnType("datetime");

                entity.Property(e => e.Qrcode).HasColumnName("QRCode");

                entity.HasOne(d => d.Crew)
                    .WithMany(p => p.OrderFulfilments)
                    .HasForeignKey(d => d.CrewId)
                    .HasConstraintName("FK_OrderFulfilments_Parties");

                entity.HasOne(d => d.DestinationLocation)
                    .WithMany(p => p.OrderFulfilmentsDestinationLocation)
                    .HasForeignKey(d => d.DestinationLocationId)
                    .HasConstraintName("FK_OrderFulfilments_DestinationLocations");

                entity.HasOne(d => d.FkWorkOrder)
                    .WithMany(p => p.OrderFulfilments)
                    .HasForeignKey(d => d.FkWorkOrderId)
                    .HasConstraintName("FK_OrderFulfilments_WorkOrders");

                entity.HasOne(d => d.PickupLocation)
                    .WithMany(p => p.OrderFulfilmentsPickupLocation)
                    .HasForeignKey(d => d.PickupLocationId)
                    .HasConstraintName("FK_OrderFulfilments_PickupLocations");
            });

            modelBuilder.Entity<Orgnization>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Country).HasMaxLength(100);

                entity.Property(e => e.MobilePhone).HasMaxLength(100);

                entity.Property(e => e.OfficePhone).HasMaxLength(100);

                entity.Property(e => e.RegCode).HasMaxLength(500);

                entity.Property(e => e.State).HasMaxLength(100);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Orgnizations)
                    .HasForeignKey<Orgnization>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orgnizations_Parties");
            });

            modelBuilder.Entity<PartyAttribute>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FkPartyId).HasColumnName("FK_Party_Id");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.ThruDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.PartyAttributes)
                    .HasForeignKey<PartyAttribute>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PartyAttributes_Parties");
            });

            modelBuilder.Entity<PartyLocation>(entity =>
            {
                entity.HasOne(d => d.Location)
                    .WithMany(p => p.PartyLocations)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK_PartyLocations_Locations");

                entity.HasOne(d => d.Party)
                    .WithMany(p => p.PartyLocations)
                    .HasForeignKey(d => d.PartyId)
                    .HasConstraintName("FK_PartyLocations_Parties");
            });

            modelBuilder.Entity<PartyRelationship>(entity =>
            {
                entity.HasOne(d => d.FromParty)
                    .WithMany(p => p.PartyRelationshipsFromParty)
                    .HasForeignKey(d => d.FromPartyId)
                    .HasConstraintName("FK_PartyRelationships_FromParties");

                entity.HasOne(d => d.FromPartyRole)
                    .WithMany(p => p.PartyRelationshipsFromPartyRole)
                    .HasForeignKey(d => d.FromPartyRoleId)
                    .HasConstraintName("FK_PartyRelationships_FromRoleTypes");

                entity.HasOne(d => d.ToParty)
                    .WithMany(p => p.PartyRelationshipsToParty)
                    .HasForeignKey(d => d.ToPartyId)
                    .HasConstraintName("FK_PartyRelationships_ToParties");

                entity.HasOne(d => d.ToPartyRole)
                    .WithMany(p => p.PartyRelationshipsToPartyRole)
                    .HasForeignKey(d => d.ToPartyRoleId)
                    .HasConstraintName("FK_PartyRelationships_ToRoleTypes");
            });

            modelBuilder.Entity<PartyRole>(entity =>
            {
                entity.Property(e => e.FkPartyId).HasColumnName("FK_Party_Id");

                entity.Property(e => e.FkRoleId).HasColumnName("FK_Role_Id");

                entity.HasOne(d => d.FkParty)
                    .WithMany(p => p.PartyRoles)
                    .HasForeignKey(d => d.FkPartyId)
                    .HasConstraintName("FK_PartyRoles_Parties");

                entity.HasOne(d => d.FkRole)
                    .WithMany(p => p.PartyRoles)
                    .HasForeignKey(d => d.FkRoleId)
                    .HasConstraintName("FK_PartyRoles_RoleTypes");
            });

            modelBuilder.Entity<People>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.City).HasMaxLength(500);

                entity.Property(e => e.Country).HasMaxLength(100);

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(500);

                entity.Property(e => e.HomePhone).HasMaxLength(100);

                entity.Property(e => e.LastName).HasMaxLength(500);

                entity.Property(e => e.MobilePhone).HasMaxLength(100);

                entity.Property(e => e.State).HasMaxLength(100);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Peoples)
                    .HasForeignKey<People>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Peoples_Parties");
            });

            modelBuilder.Entity<RoleType>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<WorkOrderItem>(entity =>
            {
                entity.HasIndex(e => e.SealNo)
                    .HasName("Unique_WorkOrderItems")
                    .IsUnique();

                entity.Property(e => e.FkWorkOrderId).HasColumnName("FK_WorkOrder_Id");

                entity.HasOne(d => d.FkWorkOrder)
                    .WithMany(p => p.WorkOrderItems)
                    .HasForeignKey(d => d.FkWorkOrderId)
                    .HasConstraintName("FK_WorkOrderItems_WorkOrders");
            });

            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ShipmentCode).HasMaxLength(500);

                entity.HasOne(d => d.FromParty)
                    .WithMany(p => p.WorkOrdersFromParty)
                    .HasForeignKey(d => d.FromPartyId)
                    .HasConstraintName("FK_WorkOrders_FromParties");

                entity.HasOne(d => d.ToParty)
                    .WithMany(p => p.WorkOrdersToParty)
                    .HasForeignKey(d => d.ToPartyId)
                    .HasConstraintName("FK_WorkOrders_ToParties");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
