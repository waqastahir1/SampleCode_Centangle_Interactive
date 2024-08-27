namespace SOS.OrderTracking.Web.Shared.Admin.MonthlyShipmentRange
{
    public class RegionRangeListModel
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string Station { get; set; }
        public string CrewOrClient { get; set; }
        public int RangeStart { get; set; }
        public int RangeEnd { get; set; }
    }
}
