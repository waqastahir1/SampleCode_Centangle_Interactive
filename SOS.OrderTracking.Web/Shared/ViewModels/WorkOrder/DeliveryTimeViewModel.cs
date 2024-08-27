using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class DeliveryTimeViewModel
    {
        public int ConsignmentId { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DropOffTime { get; set; }
    }

    public class ShipmentStatusViewModel
    {
        public int ConsignmentId { get; set; }
        public DateTime? PickupTime { get; set; }
        public ConsignmentDeliveryState ConsignmentState { get; set; }
    }
}
