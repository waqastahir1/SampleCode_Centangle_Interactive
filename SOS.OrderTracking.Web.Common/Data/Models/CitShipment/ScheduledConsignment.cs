using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ScheduledConsignment
    {
        public int Id { get; set; }
        public int ConsignmentId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual Consignment FkConsignment { get; set; }

    }
}
