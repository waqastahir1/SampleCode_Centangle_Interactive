using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingContractBranchesWisePaidLeaf
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XBranchCode { get; set; } = null!;
        public string? XBranchCodeDescription { get; set; }
        public decimal XFromDays { get; set; }
        public decimal XToDays { get; set; }
        public decimal XLeaves { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
