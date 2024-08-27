using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblSupplierInvoiceDocumentMatching
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XDocumentType { get; set; }
        public string? XDocumentTypeDescription { get; set; }
        public string? XDocumentLocation { get; set; }
        public string? XDocumentLocationDescription { get; set; }
        public string? XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime? DDate { get; set; }
        public string? XRefNumber { get; set; }
        public string? XRefDate { get; set; }
        public DateTime? DRefDate { get; set; }
        public string? XIgpNumber { get; set; }
        public string? XIgpDate { get; set; }
        public DateTime? DIgpDate { get; set; }
        public decimal? XAmount { get; set; }
        public decimal? XAlreadyAdjusted { get; set; }
        public decimal XAdjustedNow { get; set; }
        public decimal? XTotalAdjusted { get; set; }
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
