using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCitCitMonthlyOvertimeDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string? XShipment { get; set; }
        public string? XShipmentDescription { get; set; }
        public string? XManualShipmentNumbers { get; set; }
        public string XType { get; set; } = null!;
        public string? XTypeDescription { get; set; }
        public decimal XHours { get; set; }
        public decimal? XMins { get; set; }
        public string? XStartTime { get; set; }
        public DateTime? DStartTime { get; set; }
        public string? XEndTime { get; set; }
        public DateTime? DEndTime { get; set; }
        public decimal? XSalary { get; set; }
        public decimal? XAmount { get; set; }
        public string? XVehicle { get; set; }
        public string? XVehicleDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
