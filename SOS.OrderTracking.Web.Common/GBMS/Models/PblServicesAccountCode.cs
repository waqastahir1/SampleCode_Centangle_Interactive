using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblServicesAccountCode
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XAccountNature { get; set; } = null!;
        public string? XAccountNatureDescription { get; set; }
        public string XAccountCode { get; set; } = null!;
        public string? XAccountCodeDescription { get; set; }
        public string? XStandardParticulars { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
