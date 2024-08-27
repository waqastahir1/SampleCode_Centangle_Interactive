namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard
{
    public class CustomerDashboardListViewModel
    {
        public int ApprovalPendingOutgoing { get; set; }
        public int WaitingForCrewOutgoing { get; set; }
        public int InTransitOutgoing { get; set; }
        public int DeliveredOutgoing { get; set; }

        public int ApprovalPendingIncoming { get; set; }
        public int WaitingForCrewIncoming { get; set; }
        public int InTransitIncoming { get; set; }
        public int DeliveredIncoming { get; set; }
    }
}
