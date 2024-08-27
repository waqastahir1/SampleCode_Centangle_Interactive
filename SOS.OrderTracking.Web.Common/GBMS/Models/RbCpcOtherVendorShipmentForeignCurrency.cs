using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcOtherVendorShipmentForeignCurrency
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XCurrency { get; set; } = null!;
        public string? XCurrencyDescription { get; set; }
        public decimal XAmountInFc { get; set; }
        public decimal XConversionRate { get; set; }
        public decimal XAmountInPkr { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
