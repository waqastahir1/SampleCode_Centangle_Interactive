using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblVehicleFuelFilling
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
        public string? UserId { get; set; }
        public string YearCode { get; set; } = null!;
        public string? YearName { get; set; }
        public string PeriodCode { get; set; } = null!;
        public string? PeriodName { get; set; }
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public decimal XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public string XStation { get; set; } = null!;
        public string? XStationDescription { get; set; }
        public string? XPaymentType { get; set; }
        public string? XPaymentTypeDescription { get; set; }
        public string XSupplier { get; set; } = null!;
        public string? XName { get; set; }
        public string? XVehicle { get; set; }
        public string? XVehicleDescription { get; set; }
        public string? XFuelType { get; set; }
        public string? XFuelTypeDescription { get; set; }
        public string? XFuelCardNumber { get; set; }
        public string? XPortalReference { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
