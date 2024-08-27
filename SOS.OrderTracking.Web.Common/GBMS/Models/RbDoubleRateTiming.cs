using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbDoubleRateTiming
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XName { get; set; } = null!;
        public string? XAbbrevation { get; set; }
        public string? XMonday { get; set; }
        public DateTime? DMonday { get; set; }
        public string? XTuesday { get; set; }
        public DateTime? DTuesday { get; set; }
        public string? XWednesday { get; set; }
        public DateTime? DWednesday { get; set; }
        public string? XThursday { get; set; }
        public DateTime? DThursday { get; set; }
        public string? XFriday { get; set; }
        public DateTime? DFriday { get; set; }
        public string? XSaturday { get; set; }
        public DateTime? DSaturday { get; set; }
        public string? XSunday { get; set; }
        public DateTime? DSunday { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
