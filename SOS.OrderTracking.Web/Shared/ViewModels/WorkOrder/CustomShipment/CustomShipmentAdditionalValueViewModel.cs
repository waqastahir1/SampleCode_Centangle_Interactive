using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.CustomShipment
{
    public class CustomShipmentAdditionalValueViewModel : BaseIndexModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SearchKey { get; set; }
        public int CrewId { get; set; }
        public int? ConsignmentStateTypeInt { get; set; }
        public string PostingMessage { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
    }
}
