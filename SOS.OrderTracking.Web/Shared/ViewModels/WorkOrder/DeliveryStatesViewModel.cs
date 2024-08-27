using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class ConsignmentStateViewModel
    {
        public ConsignmentDeliveryState ConsignmentStateType { get; set; }

        public StateTypes Status { get; set; }

        public string Tag { get; set; }

        public DateTime? TimeStamp { get; set; }

        public int ConsignmentId { get; set; }

        public int DeliveryId { get; set; }

    }
}
