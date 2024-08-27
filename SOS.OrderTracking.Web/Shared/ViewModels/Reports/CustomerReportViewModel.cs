using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class CustomerReportViewModel
    {
        public int Id { get; set; }
        public string ShipmentCode { get; set; }
        public string PickupBranch { get; set; }
        public string DeliveryBranch { get; set; }
        public string PickupTime { get; set; }
        public string DeliveryTime { get; set; }
        public string VehicleNo { get; set; }
        public string Station { get; set; }
        public int Vault { get; set; }

        public int Amount { get; set; }
        public string AmountInWords { get; set; }
        public string CurrentDate { get; set; }
        public string CreatedBy { get; set; }
        public string OrderBy { get; set; }
        public string OrderAt { get; set; }
        public string ShipmentMedium { get; set; }
        public string Dedicated { get; set; }
        public string CollectionQR { get; set; }
        public string DeliveryQR { get; set; }
        public int SealCount { get; set; }
        public string BillTo { get; set; }
        public ConsignmentDeliveryState ConsignmentState { get; set; }
        public string State { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public string ShipmentType { get; set; }
        public string CreatedByOrgName { get; set; }
        public string ManualShipmentNo { get; set; }


        public DateTime? PlannedCollectionTime { get; set; }
        public ConsignmentDeliveryModel CollectionDelivery { get; set; }

        public ConsignmentDeliveryModel DroppoffDelivery { get; set; }
    }

    public class ConsignmentDeliveryModel
    {
        public int Id { get; set; }

        public int ConsignmentId { get; set; }
        public int? CrewId { get; set; }
        public int RegionId { get; set; }
        public int SubRegionId { get; set; }
        public int StationId { get; set; }
        public string Station { get; set; }

        public string PickupCode { get; set; }

        public int CollectionMode { get; set; }

        public int DeliveryMode { get; set; }


        public string DropoffCode { get; set; }

        public bool IsVault { get; set; }

        public DateTime? ActualPickupTime { get; set; }


        public DateTime? ActualDropTime { get; set; }

        public string Vehicle { get; set; }
        public string Crew { get; set; }

    }
}
