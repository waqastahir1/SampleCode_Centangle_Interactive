using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingContractSubRegionInvoiceRate
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XSubRegion { get; set; } = null!;
        public string? XSubRegionDescription { get; set; }
        public string XServiceCode { get; set; } = null!;
        public string? XServiceCodeDescription { get; set; }
        public string? XInvoiceType { get; set; }
        public string? XInvoiceTypeDescription { get; set; }
        public decimal XRs10 { get; set; }
        public decimal XRs20 { get; set; }
        public decimal XRs50 { get; set; }
        public decimal XRs100 { get; set; }
        public decimal XRs500 { get; set; }
        public decimal XRs1000 { get; set; }
        public decimal XRs5000 { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
