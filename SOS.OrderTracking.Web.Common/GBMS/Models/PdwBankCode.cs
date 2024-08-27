using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PdwBankCode
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XBankName { get; set; } = null!;
        public string? XAbbreviation { get; set; }
        public string? XBranchName { get; set; }
        public string? XAccountNumber { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
