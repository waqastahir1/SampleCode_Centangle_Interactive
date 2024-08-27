using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblDebitNoteEmployeeDocumentDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XReferenceNo { get; set; }
        public string? XReferenceDate { get; set; }
        public DateTime? DReferenceDate { get; set; }
        public string XDebitNoteReason { get; set; } = null!;
        public string? XDebitNoteReasonDescription { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XFurtherTax { get; set; }
        public decimal? XExtraTax { get; set; }
        public decimal? XSed { get; set; }
        public decimal? XOthers { get; set; }
        public decimal XNetValue { get; set; }
        public string XAccountCode { get; set; } = null!;
        public string? XCreditAccountCodeDescription { get; set; }
        public string? XDebitCode { get; set; }
        public string? XDebitCodeDescription { get; set; }
        public string? XCreditCode { get; set; }
        public string? XCreditCodeDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
