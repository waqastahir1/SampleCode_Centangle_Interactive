using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCpcMonthlyAttendanceDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string XCriteriaSelection { get; set; } = null!;
        public string? XCriteriaSelectionDescription { get; set; }
        public decimal XValue { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
