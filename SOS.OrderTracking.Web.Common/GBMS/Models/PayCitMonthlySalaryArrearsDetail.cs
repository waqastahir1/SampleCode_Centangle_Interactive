using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCitMonthlySalaryArrearsDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string? XSalaryDate { get; set; }
        public DateTime DSalaryDate { get; set; }
        public decimal? XPaidDays { get; set; }
        public decimal? XBasicPay { get; set; }
        public decimal? XAllowances { get; set; }
        public decimal? XDeductions { get; set; }
        public decimal? XLoansDeduction { get; set; }
        public decimal XNetPay { get; set; }
        public string? XAllowancesDetails { get; set; }
        public string? XDeductionsDetails { get; set; }
        public string? XLoansDetails { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
