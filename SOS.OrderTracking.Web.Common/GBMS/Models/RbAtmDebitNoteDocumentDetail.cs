using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmDebitNoteDocumentDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XShipmentNo { get; set; }
        public string? XShipmentNoDescription { get; set; }
        public string? XCitElement { get; set; }
        public string? XCitElementDescription { get; set; }
        public string XReason { get; set; } = null!;
        public string? XReasonDescription { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal XNetValue { get; set; }
        public string XAccountCode { get; set; } = null!;
        public string? XDebitAccountCodeDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
