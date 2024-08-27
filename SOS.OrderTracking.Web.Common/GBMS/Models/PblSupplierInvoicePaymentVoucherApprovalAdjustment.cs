using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblSupplierInvoicePaymentVoucherApprovalAdjustment
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XDocNo { get; set; }
        public string? XDate { get; set; }
        public DateTime? DDate { get; set; }
        public string? XLocation { get; set; }
        public string? XLocationDescription { get; set; }
        public decimal? XAmount { get; set; }
        public decimal? XAlreadyPaid { get; set; }
        public decimal XNowPaying { get; set; }
        public decimal? XBalance { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
