using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvProductMasterFileSpecification
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XSpecification1 { get; set; }
        public string? XSpecification2 { get; set; }
        public string? XSpecification3 { get; set; }
        public string? XDimension1 { get; set; }
        public string? XDimension2 { get; set; }
        public string? XDimension3 { get; set; }
        public string? XColour { get; set; }
        public string? XAppearance { get; set; }
        public string? XShelfLife { get; set; }
        public decimal? XLifeInMonths { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
