using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcDailySortingTransaction
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
        public string? XTranDate { get; set; }
        public DateTime? DTranDate { get; set; }
        public string? XStation { get; set; }
        public string? XStationDescription { get; set; }
        public string XMainCustomer { get; set; } = null!;
        public string? XMainCustomerDescription { get; set; }
        public string XBranch { get; set; } = null!;
        public string? XBranchDescription { get; set; }
        public string? XShipmentNumber { get; set; }
        public string? XShipmentNumberDescription { get; set; }
        public string? XShipDate { get; set; }
        public DateTime? DShipDate { get; set; }
        public string? XShipTime { get; set; }
        public DateTime? DShipTime { get; set; }
        public decimal? XShipmentAmount { get; set; }
        public decimal? XAccumulatedSorted { get; set; }
        public decimal? XBundleRs10 { get; set; }
        public decimal? XBundleRs20 { get; set; }
        public decimal? XBundleRs50 { get; set; }
        public decimal? XBundleRs100 { get; set; }
        public decimal? XBundleRs500 { get; set; }
        public decimal? XBundleRs1000 { get; set; }
        public decimal? XBundleRs5000 { get; set; }
        public decimal? XAmountRs10 { get; set; }
        public decimal? XAmountRs20 { get; set; }
        public decimal? XAmountRs50 { get; set; }
        public decimal? XAmountRs100 { get; set; }
        public decimal? XAmountRs500 { get; set; }
        public decimal? XAmountRs1000 { get; set; }
        public decimal? XAmountRs5000 { get; set; }
        public decimal? XTotalAmount { get; set; }
        public decimal? XTaxRate { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XNetAmount { get; set; }
        public string? XRevenueAuthority { get; set; }
        public string? XRevenueAuthorityDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
