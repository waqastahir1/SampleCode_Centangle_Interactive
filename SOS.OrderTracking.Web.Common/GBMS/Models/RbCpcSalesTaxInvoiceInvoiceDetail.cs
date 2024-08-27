using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcSalesTaxInvoiceInvoiceDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XCode { get; set; } = null!;
        public string? XRevenueCodeDescription { get; set; }
        public string? XDenomination { get; set; }
        public string? XDenominationDescription { get; set; }
        public decimal? XBundles { get; set; }
        public decimal? XRate { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XNetAmount { get; set; }
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
