using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Admin.MonthlyShipmentRange
{
    public class RegionRangeViewModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Region")]
        public int RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }
        public int? CrewOrClientId { get; set; }
        public int MonthlyShipmentRangeId { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string Station { get; set; }
        public string CrewOrClient { get; set; }
        public bool IsCrew { get; set; } = true;

        [Required]
        [Range(10000, int.MaxValue, ErrorMessage = "Please enter valid range starting from 10000")]
        public int RangeStart { get; set; }
        [Required]
        [Range(10000, int.MaxValue, ErrorMessage = "Please enter valid range starting from 10000")]
        public int RangeEnd { get; set; }
    }
}
