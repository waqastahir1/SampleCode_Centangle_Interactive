using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class CitGridListViewModel
    {
        public int Id { get; set; }
        public int DeliveryId { get; set; }

        public int? PreviousId { get; set; }
        public int? CrewId { get; set; }
        public string ShipmentCode { get; set; }
        public ConsignmentDeliveryState ConsignmentStateType { get; set; }
        public string CrewName { get; set; }
        public string FromPartyCode { get; set; }
        public string ToPartyCode { get; set; }
        public int Amount { get; set; }
        public int BillBranchId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ShipmentType ShipmentType { get; set; }
    }
}
