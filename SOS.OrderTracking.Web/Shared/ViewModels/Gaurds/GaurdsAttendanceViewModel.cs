using SOS.OrderTracking.Web.Shared.Enums;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Gaurds
{
    public class GaurdsAttendanceViewModel
    {
        public int RelationshipId { get; set; }

        public AttendanceState AttendanceState { get; set; }
    }
}
