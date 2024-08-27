using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ScheduleViewModel
    {
        public int ConsignmentId { get; set; }
        public byte DayOfWeek { get; set; }
        public DateTime? MondayTime { get; set; }
        public DateTime? TuesdayTime { get; set; }
        public DateTime? WednesdayTime { get; set; }
        public DateTime? ThursdayTime { get; set; }
        public DateTime? FridayTime { get; set; }
        public DateTime? SaturdayTime { get; set; }
        public DateTime? SundayTime { get; set; }
        public bool MondayScheduleExist { get; set; }
        public bool TuesdayScheduleExist { get; set; }
        public bool WednesdayScheduleExist { get; set; }
        public bool ThursdayScheduleExist { get; set; }
        public bool FridayScheduleExist { get; set; }
        public bool SaturdayScheduleExist { get; set; }
        public bool SundayScheduleExist { get; set; }
        public bool IsRecuringSchedule { get; set; }
    }
}
