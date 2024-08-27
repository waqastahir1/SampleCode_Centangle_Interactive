using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbProductClassificationCostLink
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XStockControlCode { get; set; }
        public string? XStockControlCodeDescription { get; set; }
        public string? XCostOfStock { get; set; }
        public string? XCostOfStockDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
