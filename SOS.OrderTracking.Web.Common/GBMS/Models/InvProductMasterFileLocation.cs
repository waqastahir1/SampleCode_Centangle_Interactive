using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvProductMasterFileLocation
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XWarehouse { get; set; } = null!;
        public string? XWarehouseDescription { get; set; }
        public string? XRoom { get; set; }
        public string? XRack { get; set; }
        public string? XShelf { get; set; }
        public string? XBin { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
