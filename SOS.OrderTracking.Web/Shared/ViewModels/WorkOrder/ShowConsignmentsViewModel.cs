using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class ShowConsignmentsViewModel
    {
        public string PickupBranch { get; set; }
        public string DropoffBranch { get; set; }
        public string ConsignmentStatus { get; set; }
        public int Amount { get; set; }

        public string ShipmentCode { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
