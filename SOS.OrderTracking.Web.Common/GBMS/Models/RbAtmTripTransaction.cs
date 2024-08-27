using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmTripTransaction
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
        public string? UserId { get; set; }
        public string YearCode { get; set; } = null!;
        public string? YearName { get; set; }
        public string PeriodCode { get; set; } = null!;
        public string? PeriodName { get; set; }
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public decimal XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public string XTripNo { get; set; } = null!;
        public string XTripType { get; set; } = null!;
        public string? XTripTypeDescription { get; set; }
        public string XMainCustomer { get; set; } = null!;
        public string? XMainCustomerDescription { get; set; }
        public string XAtmId { get; set; } = null!;
        public string? XAtmIdDescription { get; set; }
        public string XRevenueRegion { get; set; } = null!;
        public string? XRevenueRegionDescription { get; set; }
        public string XCustomer { get; set; } = null!;
        public string? XBillingCustomerName { get; set; }
        public string? XTripDate { get; set; }
        public DateTime DTripDate { get; set; }
        public string? XTripTime { get; set; }
        public DateTime? DTripTime { get; set; }
        public decimal XDistanceKms { get; set; }
        public decimal? XAmountFilled { get; set; }
        public decimal? XTollTax { get; set; }
        public decimal? XWaitingMins { get; set; }
        public decimal? XAdditional { get; set; }
        public decimal? XCapturedCards { get; set; }
        public string XAutoBilling { get; set; } = null!;
        public string? XRateId { get; set; }
        public string? XRateIdDescription { get; set; }
        public decimal? XFixedAmount { get; set; }
        public decimal XTripCharges { get; set; }
        public decimal XDistance { get; set; }
        public decimal XWaiting { get; set; }
        public decimal XNetAmount { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
