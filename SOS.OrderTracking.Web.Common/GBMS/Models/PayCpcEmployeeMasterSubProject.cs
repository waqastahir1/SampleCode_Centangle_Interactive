using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCpcEmployeeMasterSubProject
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XProjectCode { get; set; } = null!;
        public string? XProjectCodeDescription { get; set; }
        public string XStatus { get; set; } = null!;
        public string? XStartingDate { get; set; }
        public DateTime DStartingDate { get; set; }
        public string? XEndingDate { get; set; }
        public DateTime DEndingDate { get; set; }
        public decimal XPercentage { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
