using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbMainCustomerManagementSegmentWiseBankDetail
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XSegment { get; set; }
        public string? XSegmentDescription { get; set; }
        public string? XBankName { get; set; }
        public string? XAccountTitle { get; set; }
        public string? XAccountNumber { get; set; }
        public string? XIbanNumber { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
