using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbMainCustomerManagementCitDedicatedVehicle
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XBranch { get; set; }
        public string? XBranchDescription { get; set; }
        public decimal XTrips { get; set; }
        public decimal XRadiusKm { get; set; }
        public decimal XRate { get; set; }
        public string? XVehicles { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
