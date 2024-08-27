using SOS.OrderTracking.Web.Shared.Enums;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Common
{
    public class LocationFilterViewModel
    {
        public LocationType LocationType { get; set; }

        public int? RegionId { get; set; }

        public int? SubRegionId { get; set; }

        public int? StationId { get; set; }


    }
}
