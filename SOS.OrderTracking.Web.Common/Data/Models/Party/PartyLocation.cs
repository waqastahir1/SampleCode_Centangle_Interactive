using NetTopologySuite.Geometries;
using System;

namespace SOS.OrderTracking.Web.Common.Data.Models
{
    public class PartyLocation
    {
        public int PartyId { get; set; }

        public DateTime TimeStamp { get; set; }

        public Point Geolocation { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatdBy { get; set; }
    }
}
