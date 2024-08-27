using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbMainCustomerManagementGuardingInvoiceDay
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XMonthCode { get; set; } = null!;
        public string? XMonthCodeDescription { get; set; }
        public decimal XMonthDays { get; set; }
        public string? XStartDate { get; set; }
        public DateTime DStartDate { get; set; }
        public string? XEndDate { get; set; }
        public DateTime DEndDate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
