using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance
{
    public class DistanceHistoryListViewModel
    {
        public int Id { get; set; }
        public int FromPartyId { get; set; }

        public int ToPartyId { get; set; }
        public string FromPartyName { get; set; }

        public string ToPartyName { get; set; }

        public double Distance { get; set; }

        public DataRecordStatus DistanceStatus { get; set; }
        public DistanceSource DistanceSource { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdateAt { get; set; }
    }
}
