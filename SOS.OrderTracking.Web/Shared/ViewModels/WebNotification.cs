namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class WebNotification
    {
        public int Id { get; set; }

        public int FromPartyId { get; set; }

        public int ToPartyId { get; set; }

        public int? CollectionRegionId { get; set; }

        public int? CollectionSubRegionId { get; set; }

        public int? CollectionStationId { get; set; }

        public int? DeliveryRegionId { get; set; }

        public int? DeliverySubRegionId { get; set; }

        public int? DeliveryStationId { get; set; }

        public NotificationSource Source { get; set; }

        public enum NotificationSource
        {
            OnNewShipment,
            OnShipmentUpdated
        }
    }
}
