using NetTopologySuite.Geometries;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class DeliveryGeolog
    {
        public long TimeStamp { get; set; }

        public int DeliveryId { get; set; }

        public Point Location { get; set; }
    }
}
