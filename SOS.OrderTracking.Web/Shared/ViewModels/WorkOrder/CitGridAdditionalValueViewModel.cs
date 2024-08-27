using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class CitGridAdditionalValueViewModel : BaseIndexModel
    {
        public int CrewId { get; set; }
        public ConsignmentDeliveryState? ConsignmentStateType { get; set; }
        public int MainCustomerId { get; set; }
        public int BillBranchId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
