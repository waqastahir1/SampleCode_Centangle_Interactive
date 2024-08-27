using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class MissingShipment
    {
        [Key]
        public int Id { get; set; }

        public double ShipmentNumberProcessed { get; set; }
        public double PreviousShipmentNumberProcessed { get; set; }
        public string PreviousShipmentNumber { get; set; }
        public string PreviousShipmentPickup { get; set; }
        public string PreviousShipmentDropOff { get; set; }
        public string PreviousShipmentCity { get; set; }
        public DateTime PreviousShipmentDate { get; set; }

        public double NextShipmentNumberProcessed { get; set; }
        public string NextShipmentNumber { get; set; }
        public string NextShipmentPickup { get; set; }
        public string NextShipmentDropOff { get; set; }
        public string NextShipmentCity { get; set; }
        public DateTime NextShipmentDate { get; set; }

        public string SimilarRecordsJson { get; set; }
    }
}
