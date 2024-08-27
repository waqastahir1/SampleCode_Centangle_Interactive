using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmServiceChargesChargesRule
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XAtmElement { get; set; } = null!;
        public string? XAtmElementDescription { get; set; }
        public decimal XStartingKm { get; set; }
        public decimal XEndingKm { get; set; }
        public decimal XValue { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
