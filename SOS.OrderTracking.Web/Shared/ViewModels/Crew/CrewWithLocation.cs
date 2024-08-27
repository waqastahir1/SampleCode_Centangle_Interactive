using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewWithLocation : SelectListItem
    {
        /// <summary>
        /// Distance of Vehicle from Consignment Pickup Location
        /// </summary>
        public string PickeupStats { get; set; }

        public double PickeupStats_ { get; set; }

        /// <summary>
        /// Distance between Pickup and Dropoff
        /// </summary>
        public string ConsignmentDistance { get; set; }

        public double ConsignmentDistance_ { get; set; }

        /// <summary>
        /// No and Stats of Consignment crew has pickup up
        /// </summary>
        public string PickupUpConsignments { get; set; }

        /// <summary>
        /// No of Stats Consignment crew has to pick
        /// </summary>
        public string ToBePickedConsignments { get; set; }

        /// <summary>
        /// Distance of Colsest Distination from Consignment Dropoff location
        /// </summary>
        public string ClosetToDropffDistance { get; set; }

        public double ClosetToDropffDistance_ { get; set; }

        public TemporatlPoint CrewLocation { get; set; }

        public Point PickupLocation { get; set; }


        public IEnumerable<Point> CrewDestinations { get; set; }


    }
}

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class Point
    {
        public Point()
        {

        }

        /// <summary>
        /// Lat = Y, Long = X
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public Point(double? lat, double? lng)
        {
            Lat = lat.GetValueOrDefault();
            Lng = lng.GetValueOrDefault();
        }
        public double Lat { get; set; }

        public double Lng { get; set; }

        public override string ToString()
        {
            return $"{Lat}, {Lng}";
        }
    }

    public class TemporatlPoint : Point
    {
        public TemporatlPoint()
        {

        }

        public TemporatlPoint(double? lat, double? lng, DateTime timeStamp)
        {
            Lat = lat.GetValueOrDefault();
            Lng = lng.GetValueOrDefault();
            TimeStamp = timeStamp;
        }

        public DateTime TimeStamp { get; set; }
    }
}