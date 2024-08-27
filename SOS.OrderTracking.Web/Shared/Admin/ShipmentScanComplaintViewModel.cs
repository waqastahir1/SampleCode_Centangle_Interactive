using System;

namespace SOS.OrderTracking.Web.Shared.Admin
{
    public class ShipmentScanComplaintViewModel
    {
        public int Id { get; set; }
        public string ShipmentNumber { get; set; }
        public string Remarks { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

    }
}
