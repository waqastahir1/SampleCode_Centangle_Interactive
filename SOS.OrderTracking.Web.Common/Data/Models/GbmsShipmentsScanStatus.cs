using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class GbmsShipmentsScanStatus
    {
        [Key]
        public int Id { get; set; }
        public string ShipmentNumber { get; set; }
        public bool IsApproved { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Remarks { get; set; }
    }
}
