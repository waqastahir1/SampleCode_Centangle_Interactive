using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class MonthlyShipmentRange
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Range(10000, int.MaxValue, ErrorMessage = "Please enter valid range starting from 10000")]
        public int RangeStart { get; set; }
        [Required]
        [Range(10000, int.MaxValue, ErrorMessage = "Please enter valid range starting from 10000")]
        public int RangeEnd { get; set; }
        public DateTime MonthYear { get; set; }
        public virtual ICollection<AllocatedRange> RangeAllocatedToRegions { get; set; }

    }
}
