using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard
{
    public class SOSDashboardListViewModel
    {
        public int DedicatedShipmentCount { get; set; }
        public int DomensticShipmentCount { get; set; }
        public int LocalShipmentCount { get; set; }
        public int AtmShipmentCount { get; set; }
        public List<Shipments> MainCustomerShipmentsList { get; set; }
        public List<Shipments> RegionWiseShipmentsList { get; set; }
    }
    public class Shipments
    {
        public int ShipmentsCount { get; set; }
        public string FormalName { get; set; }
    }
}
