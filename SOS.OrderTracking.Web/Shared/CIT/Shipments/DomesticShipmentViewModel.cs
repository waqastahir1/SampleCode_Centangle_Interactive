using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.CIT.Shipments
{
    public class DomesticShipmentViewModel
    {
        public int ConsignmentId { get; set; }

        public IEnumerable<DeliveryListViewModel> Deliveries { get; set; }

        public string PickupBranchName { get; set; }

        public string PickupBranchAddress { get; set; }

        public string DropoffBranchName { get; set; }

        public string DropoffBranchAddress { get; set; }
    }
}
