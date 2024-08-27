namespace SOS.OrderTracking.Web.Shared.ViewModels.Reports
{
    public class ShipmentScanReportViewModel
    {
        public int Id { get; set; }
        public string ShipmentNumber { get; set; }
        public int ShipmentIntNumber { get; set; }
        public string Month { get; set; }
        public string MainCustomer { get; set; }
        public string ShipmentDate { get; set; }
        public string DeliveryDate { get; set; }
        public string BillingBranch { get; set; }
        public string CollectionBranch { get; set; }
        public string DeliveryBranch { get; set; }
        public string City { get; set; }
        public string ServiceType { get; set; }
        public decimal Amount { get; set; }
        public int VaultNights { get; set; }
        public string AttachmentUrl { get; set; }
        public string ShipmentType { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovalRemarks { get; set; }
    }
}
