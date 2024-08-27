using System;

namespace SOS.OrderTracking.Web.Shared.Admin.MonthlyShipmentRange
{
    public class MonthlyRangeDashboard
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public string ChildName { get; set; }
        public int RangeStart { get; set; }
        public int RangeEnd { get; set; }
        public int LastUtilizedShipment { get; set; }
        public DateTime Month { get; set; }

        public int UnUtilizedShipments { get { return RangeEnd - (LastUtilizedShipment==0?RangeStart:LastUtilizedShipment); } }
        public int UnUtilizedBooks { get { return UnUtilizedShipments / 50; } }
        public int AllocatedShipments { get { return RangeEnd - RangeStart; } }
        public int AllocatedBooks { get { return AllocatedShipments / 50; } }
    }
}
