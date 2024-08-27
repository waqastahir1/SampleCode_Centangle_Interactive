using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmServiceCharge
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XSubCode { get; set; } = null!;
        public string XName { get; set; } = null!;
        public string XTripType { get; set; } = null!;
        public string? XTripTypeDescription { get; set; }
        public string? XStartDate { get; set; }
        public DateTime DStartDate { get; set; }
        public string XMeasureDistance { get; set; } = null!;
        public string? XMeasureDistanceDescription { get; set; }
        public decimal XTripCharges { get; set; }
        public decimal XFromKms { get; set; }
        public decimal XToKms { get; set; }
        public decimal XRatePerKm { get; set; }
        public decimal XBaseDistance { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
