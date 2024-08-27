using SOS.OrderTracking.Web.Shared.Enums;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ConsignmentStatusViewModel
    {
        public int ConsignmentId { get; set; }
        public string Comments { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
    }

    public class ConsignmentApprrovalViewModel
    {
        public int ConsignmentId { get; set; }
        public string Comments { get; set; }
        public ConsignmentApprovalState ApprovalState { get; set; }
    }
}
