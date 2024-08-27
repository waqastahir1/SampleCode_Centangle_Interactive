using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblPaymentApprovalApprovalDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XDocumentLocation { get; set; }
        public string? XDocumentLocationDescription { get; set; }
        public string XDocumentType { get; set; } = null!;
        public string? XDocumentTypeDescription { get; set; }
        public string XNumber { get; set; } = null!;
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XAlreadyPaid { get; set; }
        public decimal? XBalancePayable { get; set; }
        public decimal? XNa { get; set; }
        public decimal XNowPaying { get; set; }
        public decimal XNetAdjustment { get; set; }
        public decimal? XDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
