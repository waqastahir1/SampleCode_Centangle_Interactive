using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbServiceChargesCalculationRule
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XCitElement { get; set; } = null!;
        public string? XCitElementDescription { get; set; }
        public string XCalculationRule { get; set; } = null!;
        public string? XCalculationRuleDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
