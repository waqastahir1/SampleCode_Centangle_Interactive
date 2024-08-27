using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public partial class Consignment
    {
        public Consignment()
        {
            ConsignmentDeliveries = new HashSet<ConsignmentDelivery>();
            Denominations = new HashSet<Denomination>();
            DeliveryCharges = new HashSet<ShipmentCharge>();
            ShipmentSealCodes = new HashSet<ShipmentSealCode>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string ShipmentCode { get; set; }

        [StringLength(32)]
        public string ManualShipmentCode { get; set; }

        public int MainCustomerId { get; set; }

        public int CustomerId { get; set; }

        public int FromPartyId { get; set; }

        public int ToPartyId { get; set; }

        public int BillBranchId { get; set; }


        public int CollectionRegionId { get; set; }

        public int CollectionSubRegionId { get; set; }

        public int CollectionStationId { get; set; }


        public int DeliveryRegionId { get; set; }

        public int DeliverySubRegionId { get; set; }

        public int DeliveryStationId { get; set; }


        public int BillingRegionId { get; set; }

        public int BillingSubRegionId { get; set; }

        public int BillingStationId { get; set; }

        public int OriginPartyId { get; set; }

        public int CounterPartyId { get; set; }

        public ShipmentApprovalMode ShipmentApprovalMode { get; set; }

        public ConsignmentApprovalState ApprovalState { get; set; }

        public ShipmentExecutionType Type { get; set; }

        public ConsignmentStatus ConsignmentStatus { get; set; }

        public ServiceType ServiceType { get; set; }

        public ShipmentType ShipmentType { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(450)]
        public string CreatedBy { get; set; }

        public CurrencySymbol CurrencySymbol { get; set; }

        public int Amount { get; set; }

        public int AmountPKR { get; set; }

        public double Distance { get; set; }

        public DataRecordStatus DistanceStatus { get; set; }

        public int NoOfBags { get; set; }

        public bool SealedBags { get; set; }

        public string Valueables { get; set; }

        public string Comments { get; set; }

        public decimal ExchangeRate { get; set; }

        public bool IsFinalized { get; set; }

        public DateTime? FinalizedAt { get; set; }

        [StringLength(450)]
        public string FinalizedBy { get; set; }

        public EscalationStatus EscalationStatus { get; set; }


        [Column(TypeName = "datetime")]
        public DateTime? PlanedCollectionTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActualCollectionTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PlanedDeliveryTime { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActualDeliveryTime { get; set; }

        public DateTime DueTime { get; set; }

        public Shared.Enums.ConsignmentDeliveryState ConsignmentStateType { get; set; }

        [StringLength(250)]
        public string PostingMessage { get; set; }

        public DateTime? PostedAt { get; set; }
        public byte Rating { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [StringLength(450)]
        public string ApprovedBy { get; set; }

        public int? ToChangedPartyId { get; set; }
        public bool IsToChangedPartyVerified { get; set; }
        public virtual Party MainCustomer { get; set; }

        public virtual Party Customer { get; set; }

        public virtual Party BillBranch { get; set; }

        public virtual Party FromParty { get; set; }

        public virtual Party ToParty { get; set; }

        public bool IsVault { get; set; }

        public DateTime? VaultInTime { get; set; }

        public DateTime? VaultOutTime { get; set; }


        public virtual ICollection<ConsignmentDelivery> ConsignmentDeliveries { get; set; }

        public virtual ICollection<Denomination> Denominations { get; set; }

        public virtual ICollection<ShipmentCharge> DeliveryCharges { get; set; }

        public virtual ICollection<ShipmentSealCode> ShipmentSealCodes { get; set; }
        public virtual ICollection<ScheduledConsignment> ScheduledConsignments { get; set; }
    }
}
