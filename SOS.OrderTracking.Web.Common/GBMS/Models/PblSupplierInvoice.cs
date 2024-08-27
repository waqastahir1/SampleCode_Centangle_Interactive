using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblSupplierInvoice
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
        public string XInvoiceType { get; set; } = null!;
        public string? XInvoiceTypeDescription { get; set; }
        public string XSupplier { get; set; } = null!;
        public string? XName { get; set; }
        public string XInvoice { get; set; } = null!;
        public string? XInvoiceDate { get; set; }
        public DateTime DInvoiceDate { get; set; }
        public string? XDueDate { get; set; }
        public DateTime? DDueDate { get; set; }
        public decimal XValue { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XFurtherTax { get; set; }
        public decimal? XExtraTax { get; set; }
        public decimal? XFreight { get; set; }
        public decimal? XOthers { get; set; }
        public decimal? XNetValue { get; set; }
        public decimal? XPoAdvances { get; set; }
        public string? XAdjustAllDocuments { get; set; }
        public string? XPaymentTerms { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
