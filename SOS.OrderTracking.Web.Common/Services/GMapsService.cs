using Google.Maps;
using Google.Maps.DistanceMatrix;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Linq;
using static Google.Maps.DistanceMatrix.DistanceMatrixResponse;

namespace SOS.OrderTracking.Web.Common.Services
{
    public class GMapsService
    {
        public static DistanceMatrixElement ClaculateDistanceUsinGoogle(Point p1, Point p2, string shipmentCode)
        {
            throw new InvalidShipmentDataException($"Google Service Disabled {shipmentCode}", ConsignmentStatus.DistanceIssue);

            if ((p1?.Lat).GetValueOrDefault() == 0 || (p2?.Lat).GetValueOrDefault() == 0)
                throw new InvalidShipmentDataException($"Lat/long missing {shipmentCode}", ConsignmentStatus.DistanceIssue);

            if (p1.Lat == p2.Lat && p1.Lng == p2.Lng)
            {
                //throw new InvalidShipmentDataException($"Same Lat/long for collection/delivery {shipmentCode}", ConsignmentStatus.DistanceIssue);
                return new DistanceMatrixElement()
                {
                    distance = new ValueText()
                    {
                        Text = "1",
                        Value = 1
                    },
                    duration = new ValueText(),
                    Status = ServiceResponseStatus.Ok
                };
            }

            throw new InvalidShipmentDataException($"Google Service inactive {shipmentCode}", ConsignmentStatus.DistanceIssue);

            var request = new DistanceMatrixRequest();

            request.AddOrigin(new Google.Maps.Location($"{p1.Lat},{p1.Lng}"));
            request.AddDestination(new Google.Maps.Location($"{p2.Lat},{p2.Lng}"));

            var service = new DistanceMatrixService();

            var response = service.GetResponse(request);


            if (response.Status != ServiceResponseStatus.Ok)
                throw new InvalidShipmentDataException($"Google Service Error {shipmentCode}", ConsignmentStatus.DistanceIssue);

            if (response.Rows.FirstOrDefault()?.Elements.FirstOrDefault()?.Status != ServiceResponseStatus.Ok)
                throw new InvalidShipmentDataException($"Lat/long invalid {shipmentCode}", ConsignmentStatus.DistanceIssue);

            return response.Rows.First().Elements.OrderBy(x => x.distance).First();
        }
    }
}
