using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvProductClassificationRate
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public decimal? XStandardCost { get; set; }
        public decimal? XWholesaleRate { get; set; }
        public decimal? XSellingRate { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XFurtherTax { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
