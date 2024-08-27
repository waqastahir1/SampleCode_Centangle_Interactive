using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class AllocatedRange
    {
        [Key]
        public int Id { get; set; }
        public int RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }
        public int? CrewOrClientId { get; set; }
        public bool? IsCrew { get; set; }
        public bool IsReturned { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        [Required]
        [Range(10000, int.MaxValue, ErrorMessage = "Please enter valid range starting from 10000")]
        public int RangeStart { get; set; }
        [Required]
        [Range(10000, int.MaxValue, ErrorMessage = "Please enter valid range starting from 10000")]
        public int RangeEnd { get; set; }
        public int MonthlyRangeId { get; set; }
        [ForeignKey("MonthlyRangeId")]
        public MonthlyShipmentRange MonthlyShipmentRange { get; set; }

    }
}
