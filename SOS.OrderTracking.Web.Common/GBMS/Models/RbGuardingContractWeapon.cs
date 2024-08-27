using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingContractWeapon
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XWeaponCode { get; set; } = null!;
        public string? XWeaponCodeDescription { get; set; }
        public decimal XQuantity { get; set; }
        public decimal XRate { get; set; }
        public decimal XAmount { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
