using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbDoubleRateTimingsCitRule
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XWeekday { get; set; } = null!;
        public string? XWeekdayDescription { get; set; }
        public string XElement { get; set; } = null!;
        public string? XElementDescription { get; set; }
        public decimal XAmountFrom { get; set; }
        public decimal XAmountTo { get; set; }
        public decimal XIncreasePercentage { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
