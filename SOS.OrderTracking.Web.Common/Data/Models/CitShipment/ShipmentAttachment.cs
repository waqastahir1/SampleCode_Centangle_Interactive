using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.Data.Models.CitShipment
{
    public class ShipmentAttachment
    {
        [Key]
        public int Id { get; set; }
        public int ConsignmentId { get; set; }
        public string Url { get; set; }
    }
}
