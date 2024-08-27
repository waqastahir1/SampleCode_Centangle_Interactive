using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcSalesTaxInvoiceSortingDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XShipmentNumber { get; set; }
        public string? XDstNumber { get; set; }
        public string? XBranch { get; set; }
        public string? XBranchDescription { get; set; }
        public decimal? XTotalAmount { get; set; }
        public decimal? XAmountRs10 { get; set; }
        public decimal? XAmountRs20 { get; set; }
        public decimal? XAmountRs50 { get; set; }
        public decimal? XAmountRs100 { get; set; }
        public decimal? XAmountRs500 { get; set; }
        public decimal? XAmountRs1000 { get; set; }
        public decimal? XAmountRs5000 { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
