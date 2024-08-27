using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCitEmployeeMasterLoan
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XLoanSelection { get; set; } = null!;
        public string? XLoanSelectionDescription { get; set; }
        public string? XDescription { get; set; }
        public string XStatus { get; set; } = null!;
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public string? XEffectiveDate { get; set; }
        public DateTime? DEffectiveDate { get; set; }
        public decimal XPrincipal { get; set; }
        public decimal XInstallment { get; set; }
        public decimal? XInterest { get; set; }
        public decimal? XPaidBack { get; set; }
        public decimal? XLumpSum { get; set; }
        public decimal? XBalance { get; set; }
        public string? XApprovalNumber { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
