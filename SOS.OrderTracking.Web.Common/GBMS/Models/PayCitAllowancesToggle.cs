using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCitAllowancesToggle
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XToggleCode { get; set; } = null!;
        public string XDescription { get; set; } = null!;
        public string? XShortHeading { get; set; }
        public string XType { get; set; } = null!;
        public decimal? XAmount { get; set; }
        public decimal? XFormula { get; set; }
        public string XTaxable { get; set; } = null!;
        public string XAttendanceFactor { get; set; } = null!;
        public string? XExpiryDate { get; set; }
        public DateTime? DExpiryDate { get; set; }
        public decimal? XMinimum { get; set; }
        public decimal? XMaximum { get; set; }
        public decimal? XPercent { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
