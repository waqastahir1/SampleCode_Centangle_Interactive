using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.APIs.Models
{
    public class GeoCoordinate : IEqualityComparer<GeoCoordinate>
    {
        public double? Lat { get; set; }

        public double? Long_ { get; set; }

        public DateTime? TimeStamp { get; set; }

        public bool Equals(GeoCoordinate x, GeoCoordinate y)
        {
            return x.Lat == y.Lat && x.Long_ == y.Long_;
        }

        public int GetHashCode([DisallowNull] GeoCoordinate obj)
        {
            throw new NotImplementedException();
        }
    }
}
