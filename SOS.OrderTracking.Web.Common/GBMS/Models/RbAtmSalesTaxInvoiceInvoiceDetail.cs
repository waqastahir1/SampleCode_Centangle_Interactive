using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmSalesTaxInvoiceInvoiceDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XCode { get; set; } = null!;
        public string? XRevenueCodeDescription { get; set; }
        public decimal XGrossValue { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal XNetReceivable { get; set; }
        public string? XParticulars { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
