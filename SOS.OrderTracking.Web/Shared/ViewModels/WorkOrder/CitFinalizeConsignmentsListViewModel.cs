using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class CitFinalizeConsignmentsListViewModel
    {
        public int ConsignmentId { get; set; }
        public int DeliveryId { get; set; }
        public int? PreviousId { get; set; }
        public int? CrewId { get; set; }
        public string ShipmentCode { get; set; }
        public string ManualShipmentCode { get; set; }
        public ConsignmentDeliveryState ConsignmentStateType { get; set; }
        public string CrewName { get; set; }
        public int FromPartyId { get; set; }
        public string FromPartyCode { get; set; }
        public string FromPartyName { get; set; }
        public DataRecordStatus FromPartyGeoStatus { get; set; }
        public int ToPartyId { get; set; }
        public string ToPartyName { get; set; }
        public string ToPartyCode { get; set; }
        public DataRecordStatus ToPartyGeoStatus { get; set; }
        public int Amount { get; set; }
        public int BillBranchId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool Selected { get; set; }
        public string BillBranchName { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public string Comments { get; set; }
        public ShipmentType ShipmentType { get; set; }
        public DataRecordStatus DistanceStatus { get; set; }
        public double Distance { get; set; }

    }
}
