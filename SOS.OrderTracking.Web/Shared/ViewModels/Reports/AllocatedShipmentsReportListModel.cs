using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Reports
{
    public class AllocatedShipmentsReportListModel
    {
        public int Id { get; set; }
        public DateTime ForMonth { get; set; }
        public string ForMonthString { get { return ForMonth.ToString("MMM, yyyy"); } }
        public int RegionId { get; set; }
        public int SubRegionId { get; set; }
        public int StationId { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string Station { get; set; }
        public string CrewOrClient { get; set; }
        public int RangeStart { get; set; }
        public int RangeEnd { get; set; }
        public bool? isCrew { get; set; }

        public DateTime AllocationDate { get; set; }
        public string AllocationDateString { get { return AllocationDate.ToString("dd-MM-yyyy"); } }
    }
}
