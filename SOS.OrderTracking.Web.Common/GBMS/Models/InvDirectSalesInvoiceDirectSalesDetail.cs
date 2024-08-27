using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvDirectSalesInvoiceDirectSalesDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XCode { get; set; } = null!;
        public string? XProduct { get; set; }
        public string XUom { get; set; } = null!;
        public string? XBatchNumber { get; set; }
        public string? XBatchNumberDescription { get; set; }
        public decimal XQuantity { get; set; }
        public decimal XRate { get; set; }
        public decimal XGrossAmount { get; set; }
        public decimal? XDiscPercentage { get; set; }
        public decimal? XDiscount { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XAddSTax { get; set; }
        public decimal? XAdvITax { get; set; }
        public decimal? XSed { get; set; }
        public decimal XNetAmount { get; set; }
        public string? XOtherDetails { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
