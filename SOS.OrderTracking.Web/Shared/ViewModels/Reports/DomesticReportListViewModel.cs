using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;
using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Reports
{
    public class DomesticReportListViewModel
    {
        public int Id { get; set; }
        public string Client { get; set; }
        public string ShipmentCode { get; set; }
        public string PickupStation { get; set; }
        public string DeliveryStation { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string Vehicle { get; set; }
        public string VehicleStation { get; set; }
        public string VehicleCC { get; set; }
        public string DropVehicle { get; set; }
        public int Amount { get; set; }
        public int NoOfBags { get; set; }
        public int NoOfSeals { get; set; }
        public List<string> SealNumbers { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public ConsignmentDeliveryState ConsignmentState { get; set; }
        public string ShipmentType { get; set; }

        public ConsignmentDeliveryModel CollectionDelivery { get; set; }
        public ConsignmentDeliveryModel DroppoffDelivery { get; set; }
    }
}
