using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class CancelledShipment
    {
        [Key]
        public int Id { get; set; }
        public int ShipmentNumber { get; set; }
        public string Remarks { get; set; }
    }
}
