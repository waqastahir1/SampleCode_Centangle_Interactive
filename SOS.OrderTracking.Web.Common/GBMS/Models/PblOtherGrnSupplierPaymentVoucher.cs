using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblOtherGrnSupplierPaymentVoucher
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
        public string? XProject { get; set; }
        public string? XProjectDescription { get; set; }
        public string XSupplier { get; set; } = null!;
        public string? XName { get; set; }
        public string XPaymentType { get; set; } = null!;
        public string? XPaymentTypeDescription { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XAdvITax { get; set; }
        public decimal? XAdvSTax { get; set; }
        public decimal? XAnyDeductions { get; set; }
        public decimal XNetPayment { get; set; }
        public string XPayAllDocuments { get; set; } = null!;
        public string? XManualRefNo { get; set; }
        public string XAccount { get; set; } = null!;
        public string? XBankOrCashDescription { get; set; }
        public string XInstrumentType { get; set; } = null!;
        public string? XInstrumentTypeDescription { get; set; }
        public string? XInstrumentNo { get; set; }
        public string? XInstDate { get; set; }
        public DateTime? DInstDate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
