using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayGuardingMonthlyAttendanceBranchWiseDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string XBranch { get; set; } = null!;
        public string? XBranchDescription { get; set; }
        public string? XBranchStatus { get; set; }
        public string? XBranchStatusDescription { get; set; }
        public string XType { get; set; } = null!;
        public string? XTypeDescription { get; set; }
        public decimal XValue { get; set; }
        public decimal? XMonthlyRate { get; set; }
        public decimal? XAmount { get; set; }
        public string? XDesignation { get; set; }
        public string? XDesignationDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
