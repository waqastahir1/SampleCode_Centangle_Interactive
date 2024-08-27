using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblVendorClassAccountCode
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XNatureOfAccount { get; set; } = null!;
        public string? XNatureOfAccountDescription { get; set; }
        public string XAccountCode { get; set; } = null!;
        public string? XAccountCodeDescription { get; set; }
        public string? XParticularsForGlEntry { get; set; }
        public string? XRemrks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
