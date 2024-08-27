using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ShipmentSealCode
    {
        public int ConsignmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string SealCode { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public Consignment Consignment { get; set; }

    }
}
