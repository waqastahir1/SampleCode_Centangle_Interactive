using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvPurchaseDemandSpecification
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public decimal? MDetailId { get; set; }
        public decimal? XSerial { get; set; }
        public string? XSpcification { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
