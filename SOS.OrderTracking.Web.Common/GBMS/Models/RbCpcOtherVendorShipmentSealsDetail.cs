using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcOtherVendorShipmentSealsDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XSealCode { get; set; } = null!;
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
