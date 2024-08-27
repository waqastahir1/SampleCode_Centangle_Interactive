using SOS.OrderTracking.Web.Shared.Enums;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings
{
    public class RatingCategoriesViewModel
    {
        public int ConsignmentId { get; set; }
        public int ComplaintId { get; set; }
        public string Description { get; set; }
        public bool IsBadBehaviour { get; set; }
        public bool IsBadQuality { get; set; }
        public bool IsShipmentDelayed { get; set; }
        public ComplaintStatus Status { get; set; }
        public int RatingValue { get; set; }
    }
}
