using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class CitFinalizeConsignmentsAdditionalValueModel : BaseIndexModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SearchKey { get; set; }
        public int CrewId { get; set; }
        public ConsignmentDeliveryState? ConsignmentStateType { get; set; }

        public ConsignmentStatus ConsignmentStatus { get; set; }
    }
}
