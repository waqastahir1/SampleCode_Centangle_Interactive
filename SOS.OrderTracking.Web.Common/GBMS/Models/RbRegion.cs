using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbRegion
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XDescription { get; set; } = null!;
        public string? XAbbrevation { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
