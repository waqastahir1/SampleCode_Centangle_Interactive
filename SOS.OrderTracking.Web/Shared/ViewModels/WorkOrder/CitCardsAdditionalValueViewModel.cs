using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class CitCardsAdditionalValueViewModel : BaseIndexModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }

        public ShipmentExecutionType ConsignmentType { get; set; }

        public ConsignmentDeliveryState ConsignmentStateSummarized { get; set; }


        public ShipmentType ShipmentType { get; set; }

        public string SearchKey { get; set; }

        public int Rating { get; set; }

        public int Id { get; set; }

        public SortBy Sorting { get; set; }
        public int MainCustomerId { get; set; }


    }
}
