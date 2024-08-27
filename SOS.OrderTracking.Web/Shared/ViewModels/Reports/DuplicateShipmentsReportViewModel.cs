namespace SOS.OrderTracking.Web.Shared.ViewModels.Reports
{
    public class DuplicateShipmentsReportViewModel
    {
        public double ShipmentNo { get; set; }
        public DuplicateShipment[] DuplicateShipments { get; set; }
    }

    public class DuplicateShipment
    {
        public double ShipmentNo { get; set; }
        public string ShipmentNumber { get; set; }
        public string Month { get; set; }
        public string MainCustomer { get; set; }
        public string ShipmentDate { get; set; }
        public string BillingBranch { get; set; }
        public string CollectionBranch { get; set; }
        public string DeliveryBranch { get; set; }
        public string City { get; set; }
        public string ServiceType { get; set; }
        public decimal Amount { get; set; }
        public decimal? VaultNights { get; set; }
    }
}
