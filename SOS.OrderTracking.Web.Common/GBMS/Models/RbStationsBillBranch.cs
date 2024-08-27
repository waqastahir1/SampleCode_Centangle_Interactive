using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbStationsBillBranch
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XCustomer { get; set; } = null!;
        public string? XCustomerDescription { get; set; }
        public string XBranch { get; set; } = null!;
        public string? XBranchDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
