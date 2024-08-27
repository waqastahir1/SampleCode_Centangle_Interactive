using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Enums.CPC;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class CPCService
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string ShipmentCode { get; set; }

        public int? CitShipmentId { get; set; }

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


        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(450)]
        public string CreatedBy { get; set; }

        public CurrencySymbol CurrencySymbol { get; set; }

        public int AmountByCustomer { get; set; }

        public int AmountBySos { get; set; }

        public string Comments { get; set; }

        public decimal ExchangeRate { get; set; }

        public DateTime DueTime { get; set; }

        public ConsignmentApprovalState ApprovalState { get; set; }

        public ConsignmentStatus ConsignmentStatus { get; set; }

        public CpcServiceCategory CpcServiceCategory { get; set; }

        public CPCConsignmentDisposalState CPCConsignmentDisposalState { get; set; }
        public CPCConsignmentProcessingState CPCConsignmentProcessingState { get; set; }

        [StringLength(250)]
        public string PostingMessage { get; set; }

        public DateTime? PostedAt { get; set; }

        public byte Rating { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [StringLength(450)]
        public string ApprovedBy { get; set; }
        public bool IsFinalized { get; set; }

        public int CurrencyByCustomer10x { get; set; }
        public int CurrencyByCustomer20x { get; set; }
        public int CurrencyByCustomer50x { get; set; }
        public int CurrencyByCustomer75x { get; set; }
        public int CurrencyByCustomer100x { get; set; }
        public int CurrencyByCustomer500x { get; set; }
        public int CurrencyByCustomer1000x { get; set; }
        public int CurrencyByCustomer5000x { get; set; }
        public int CurrencyByCustomer200x { get; set; }
        public int CurrencyByCustomer750x { get; set; }
        public int CurrencyByCustomer1500x { get; set; }
        public int CurrencyByCustomer7500x { get; set; }
        public int CurrencyByCustomer15000x { get; set; }
        public int CurrencyByCustomer25000x { get; set; }
        public int CurrencyByCustomer40000x { get; set; }

        public int PrizeMoney100x { get; set; }
        public int PrizeMoney200x { get; set; }
        public int PrizeMoney750x { get; set; }
        public int PrizeMoney1500x { get; set; }
        public int PrizeMoney7500x { get; set; }
        public int PrizeMoney15000x { get; set; }
        public int PrizeMoney25000x { get; set; }
        public int PrizeMoney40000x { get; set; }
        public DenominationType DenominationTypeByCustomer { get; set; }

        public int CurrencyBySos10x { get; set; }
        public int CurrencyBySos20x { get; set; }
        public int CurrencyBySos50x { get; set; }
        public int CurrencyBySos75x { get; set; }
        public int CurrencyBySos100x { get; set; }
        public int CurrencyBySos500x { get; set; }
        public int CurrencyBySos1000x { get; set; }
        public int CurrencyBySos5000x { get; set; }
        public int CurrencyBySos200x { get; set; }
        public int CurrencyBySos750x { get; set; }
        public int CurrencyBySos1500x { get; set; }
        public int CurrencyBySos7500x { get; set; }
        public int CurrencyBySos15000x { get; set; }
        public int CurrencyBySos25000x { get; set; }
        public int CurrencyBySos40000x { get; set; }
        public DenominationType DenominationTypeBySos { get; set; }

        public string ReceivedBy { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public byte ReceivingMode { get; set; }

        #region Amounts

        public int ProcessedAmount { get; set; }

        public int DisposedAmount { get; set; }

        public int Balance { get; set; }

        public int ProcessedBalance { get; set; }

        public int UnprocessedBalance { get; set; }

        #endregion
    }
}
