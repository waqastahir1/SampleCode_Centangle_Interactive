using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvPurchaseOrderPaymentTerm
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public decimal XSerial { get; set; }
        public decimal? XSubSerial { get; set; }
        public string? XPaymentTermsText { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
