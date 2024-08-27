namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class DistanceReportListViewModel
    {
        public int Id { get; set; }
        public string ShipmentCode { get; set; }
        public string PickupBranch { get; set; }
        public string DeliveryBranch { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public string Distance { get; set; }
        public string DistanceStatus { get; set; }
        public string ConsignmentState { get; set; }
        public string ConsignmentStatus { get; set; }
        public string ShipmentType { get; set; }
        public string CreatedByOrgName { get; set; }
        public string PostingMessages { get; set; }
    }
}
