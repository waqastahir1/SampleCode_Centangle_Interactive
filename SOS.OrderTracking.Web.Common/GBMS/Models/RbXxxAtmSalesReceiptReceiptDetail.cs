using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbXxxAtmSalesReceiptReceiptDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XType { get; set; } = null!;
        public string XNumber { get; set; } = null!;
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XAlreadyRcvd { get; set; }
        public decimal XReceivedNow { get; set; }
        public decimal? XNetReceived { get; set; }
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
