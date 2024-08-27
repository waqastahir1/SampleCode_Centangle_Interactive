using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.AdditionRequests
{
    public class PendingRequestsListViewModel
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ThruDate { get; set; }
        public RequestStatus RequestStatus { get; set; }

        public int RequestedById { get; set; } // Bank/Branch Id

        public DateTime RequestedAt { get; set; }
    }
}
