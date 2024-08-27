using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingAdditionalGuardRequestGuardDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XBranchCode { get; set; } = null!;
        public string? XBranchCodeDescription { get; set; }
        public string XServiceType { get; set; } = null!;
        public string? XServiceTypeDescription { get; set; }
        public decimal XNoOfGuards { get; set; }
        public string XServiceCode { get; set; } = null!;
        public string? XServiceCodeDescription { get; set; }
        public string XRequestType { get; set; } = null!;
        public string? XRequestTypeDescription { get; set; }
        public string? XRequestDate { get; set; }
        public DateTime DRequestDate { get; set; }
        public string? XEndDate { get; set; }
        public DateTime? DEndDate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
