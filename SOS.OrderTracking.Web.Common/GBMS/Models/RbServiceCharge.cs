using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbServiceCharge
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XSubCode { get; set; } = null!;
        public string XName { get; set; } = null!;
        public string XShipmentType { get; set; } = null!;
        public string? XShipmentTypeDescription { get; set; }
        public string? XStartDate { get; set; }
        public DateTime DStartDate { get; set; }
        public string XMeasureDistance { get; set; } = null!;
        public string? XMeasureDistanceDescription { get; set; }
        public decimal XBaseLimit { get; set; }
        public decimal XBaseCharges { get; set; }
        public decimal? XSurCharges { get; set; }
        public decimal XVaultCharges { get; set; }
        public decimal XSealCharges { get; set; }
        public decimal XFromKms { get; set; }
        public decimal XToKms { get; set; }
        public decimal XRatePerKm { get; set; }
        public decimal XBaseDistance { get; set; }
        public string? XVaultRules { get; set; }
        public string? XVaultRulesDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
