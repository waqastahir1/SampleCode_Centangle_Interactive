namespace SOS.OrderTracking.Web.Common.Services.Dashboards
{
    public class BankHeadOfficeDashboardListingModel
    {

        public int ApprovalPending { get; set; }
        public int WaitingForCrew { get; set; }
        public int InTransit { get; set; }
        public int Delivered { get; set; }
    }
}
