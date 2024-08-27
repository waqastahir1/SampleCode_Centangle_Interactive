using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblPaymentApprovalDebitNoteAdjustment
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XDebitNoteLocation { get; set; }
        public string? XDebitNoteLocationDescription { get; set; }
        public string? XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime? DDate { get; set; }
        public decimal? XAmount { get; set; }
        public decimal? XAlreadyAdjusted { get; set; }
        public decimal? XAdjustedNow { get; set; }
        public decimal? XNetAdjustment { get; set; }
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
