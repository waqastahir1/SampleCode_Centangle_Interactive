using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewAttendanceFormViewModel
    {
        public int RelationshipId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public AttendanceState? AttendanceState { get; set; }
    }
}
