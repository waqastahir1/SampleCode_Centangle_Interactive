using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbMainCustomerManagementCitBranchRate
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XFrom { get; set; } = null!;
        public string? XFromDescription { get; set; }
        public string XDestination { get; set; } = null!;
        public string? XDestinationDescription { get; set; }
        public string? XValidFrom { get; set; }
        public DateTime DValidFrom { get; set; }
        public string? XValidTo { get; set; }
        public DateTime DValidTo { get; set; }
        public string XStatus { get; set; } = null!;
        public string? XStatusDescription { get; set; }
        public decimal XShipmentRate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
