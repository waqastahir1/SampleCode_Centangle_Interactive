using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class TransitTimeViewModel
    {
        public int ConsignmentId { get; set; }
        public bool IsCrewAssigned { get; set; }
        public List<TransitTime> ListOfTransitTime { get; set; }

    }
    public class TransitTime
    {
        public DateTime? ActualPickupTime { get; set; }
        public ConsignmentDeliveryState DeliveryState { get; set; }
        public int CrewId { get; set; }
        public string CrewName { get; set; }
        public int DeliveryId { get; set; }
        public Point CollectionPoint { get; set; }

    }
}
