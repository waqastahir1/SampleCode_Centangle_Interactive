using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayMgtLocation
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XDescription { get; set; } = null!;
        public string? XAbbreviation { get; set; }
        public string? XManagerTitle { get; set; }
        public string? XBankAndBranchName { get; set; }
        public string? XAccountNumber { get; set; }
        public string? XSignatoryName { get; set; }
        public string? XSignatoryDesignation { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
