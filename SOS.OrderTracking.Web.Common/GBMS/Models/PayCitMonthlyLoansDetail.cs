using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCitMonthlyLoansDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string XLoanSelection { get; set; } = null!;
        public string? XLoanSelectionDescription { get; set; }
        public string? XLoanDate { get; set; }
        public DateTime DLoanDate { get; set; }
        public string? XEffectiveDate { get; set; }
        public DateTime DEffectiveDate { get; set; }
        public string? XApprovalNumber { get; set; }
        public decimal XPrincipal { get; set; }
        public decimal XInstallment { get; set; }
        public decimal? XInterest { get; set; }
        public decimal XTotalLoan { get; set; }
        public decimal XPaidBack { get; set; }
        public decimal XBalance { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
