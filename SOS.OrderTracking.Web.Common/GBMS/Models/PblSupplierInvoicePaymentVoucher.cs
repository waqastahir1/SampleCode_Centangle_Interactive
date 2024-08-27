using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblSupplierInvoicePaymentVoucher
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
        public string? UserId { get; set; }
        public string YearCode { get; set; } = null!;
        public string? YearName { get; set; }
        public string PeriodCode { get; set; } = null!;
        public string? PeriodName { get; set; }
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public decimal XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public string XSupplier { get; set; } = null!;
        public string? XName { get; set; }
        public string? XManualRefNo { get; set; }
        public decimal XGrossAmount { get; set; }
        public decimal? XWht { get; set; }
        public decimal? XStwh { get; set; }
        public decimal? XOtherDed { get; set; }
        public decimal XNetPayment { get; set; }
        public string? XAccountTitle { get; set; }
        public string? XBankAccountName { get; set; }
        public string? XBankName { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
