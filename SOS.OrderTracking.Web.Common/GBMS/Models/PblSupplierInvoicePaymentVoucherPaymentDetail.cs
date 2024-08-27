using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblSupplierInvoicePaymentVoucherPaymentDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XAccount { get; set; } = null!;
        public string? XAccountDescription { get; set; }
        public string XInstrumentType { get; set; } = null!;
        public string? XInstrumentTypeDescription { get; set; }
        public string? XInstrumentNo { get; set; }
        public string? XInstDate { get; set; }
        public DateTime? DInstDate { get; set; }
        public decimal XAmount { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
