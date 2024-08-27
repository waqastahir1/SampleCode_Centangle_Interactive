namespace SOS.OrderTracking.Web.Common.ViewModels.Order
{
    public class OrderChargesViewModel
    {
        public int BaseCharges { get; set; }
        public int Surcharge { get; set; }
        public int Wallet { get; set; }
        public int AdditionalCharges { get; set; }
        public int SealCharges { get; set; }
        public int OverTimeCharges { get; set; }
        public int DistanceCharges { get; set; }
        public int ExtraCharges { get; set; }
        public int WaitingCharges { get; set; }
        public int TollCharges { get; set; }
        public int WorkOrderId { get; set; }
    }
}
