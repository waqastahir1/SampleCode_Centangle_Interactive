using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvDemandDecisionTransferLocation
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public decimal? MDetailId { get; set; }
        public string XWarehouse { get; set; } = null!;
        public string? XWarehouseDescription { get; set; }
        public decimal XQuantity { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
