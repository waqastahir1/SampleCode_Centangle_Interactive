using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class ShipmentDeliveryApiLog
    {
        public int ShipmentDeliveryId { get; set; }

        public Shared.Enums.ConsignmentDeliveryState ShipmentState { get; set; }

        public bool Status { get; set; }

        public DateTime TimeStamp { get; set; }

        public byte RretryCount { get; set; }

        [StringLength(maximumLength: 400)]
        public string JsonContent { get; set; }

        [StringLength(maximumLength: 400)]
        public string Tag { get; set; }
    }
}
