using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvWarehouse
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string? XWhCode { get; set; }
        public string? XMasterLocation { get; set; }
        public string? XMasterLocationDescription { get; set; }
        public string XDescription { get; set; } = null!;
        public decimal? XGrndLimit { get; set; }
        public decimal? XMonthlyLimit { get; set; }
        public string? XStatus { get; set; }
        public string? XStatusDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
