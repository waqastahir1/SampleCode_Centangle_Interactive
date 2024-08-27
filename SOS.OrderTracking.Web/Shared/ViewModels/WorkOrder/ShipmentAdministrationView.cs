using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ShipmentAdministrationViewModel
    {
        public ShipmentAdministrationViewModel()
        {

        }
        public ShipmentAdministrationViewModel(int id, double distance, DataRecordStatus distanceStatus)
        {
            ConsignmentId = id;
            Distance = distance;
            DistanceStatus = distanceStatus;
        }
        public int ConsignmentId { get; set; }

        [Range(minimum: 0.01, maximum: 10000)]
        public double Distance { get; set; }

        public DataRecordStatus DistanceStatus { get; set; }

    }
}
