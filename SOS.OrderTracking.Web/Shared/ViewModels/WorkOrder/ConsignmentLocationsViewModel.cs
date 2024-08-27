namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ConsignmentLocationsViewModel
    {
        public Point PickupPoint { get; set; }

        public int PickupPartyId { get; set; }

        public int? PickupStationId { get; set; }

        public Point DropoffPoint { get; set; }

        public int DropoffPartyId { get; set; }

        public int DeliveryId { get; set; }
        public int? DropoffStationId { get; set; }

        public string FromPartyCode { get; set; }


        public string ToPartyCode { get; set; }


    }
}
