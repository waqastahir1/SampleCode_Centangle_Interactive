using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class GemExpenseDisbursementVoucherDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string? XLocationName { get; set; }
        public string? XLocationNameDescription { get; set; }
        public string? XDocumentType { get; set; }
        public string? XDocumentTypeDescription { get; set; }
        public string? XDocument { get; set; }
        public string? XDate { get; set; }
        public decimal XPayableAmount { get; set; }
        public decimal XAlreadyPaid { get; set; }
        public decimal XPaidNow { get; set; }
        public decimal XBalance { get; set; }
        public string? XAccountTitle { get; set; }
        public string? XBankAccountNumber { get; set; }
        public string? XBankName { get; set; }
        public string? XBankNameDescription { get; set; }
        public string? XBeneficiaryAccountTitle { get; set; }
        public string? XBeneficiaryAccountNumber { get; set; }
        public string? XBeneficiaryMobileNumber { get; set; }
        public string? XBeneficiaryBankName { get; set; }
        public string? XBeneficiaryBankNameDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
