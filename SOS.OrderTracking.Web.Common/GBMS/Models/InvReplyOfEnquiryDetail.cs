using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvReplyOfEnquiryDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XItemCode { get; set; } = null!;
        public string? XDescription { get; set; }
        public string? XUom { get; set; }
        public decimal XQuantity { get; set; }
        public decimal XRate { get; set; }
        public decimal XValue { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XFurtherTax { get; set; }
        public decimal? XExtraTax { get; set; }
        public decimal? XFreight { get; set; }
        public decimal? XOtherCosts { get; set; }
        public decimal XNetValue { get; set; }
        public string? XMake { get; set; }
        public string? XModel { get; set; }
        public string? XPurchaseOrderSelection { get; set; }
        public string? XProject { get; set; }
        public string? XProjectDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
