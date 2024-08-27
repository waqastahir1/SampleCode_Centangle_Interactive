using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcDailySortingTransactionsSortingDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public decimal XTargetAmount { get; set; }
        public decimal? XSortedAmount { get; set; }
        public decimal? XLeafRs10 { get; set; }
        public decimal? XLeafRs20 { get; set; }
        public decimal? XLeafRs50 { get; set; }
        public decimal? XLeafRs100 { get; set; }
        public decimal? XLeafRs500 { get; set; }
        public decimal? XLeafRs1000 { get; set; }
        public decimal? XLeafRs5000 { get; set; }
        public decimal? XPacketRs10 { get; set; }
        public decimal? XPacketRs20 { get; set; }
        public decimal? XPacketRs50 { get; set; }
        public decimal? XPacketRs100 { get; set; }
        public decimal? XPacketRs500 { get; set; }
        public decimal? XPacketRs1000 { get; set; }
        public decimal? XPacketRs5000 { get; set; }
        public decimal? XBundleRs10 { get; set; }
        public decimal? XBundleRs20 { get; set; }
        public decimal? XBundleRs50 { get; set; }
        public decimal? XBundleRs100 { get; set; }
        public decimal? XBundleRs500 { get; set; }
        public decimal? XBundleRs1000 { get; set; }
        public decimal? XBundleRs5000 { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
