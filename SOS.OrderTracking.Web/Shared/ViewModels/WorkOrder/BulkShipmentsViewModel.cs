using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{
    public class BulkShipmentsViewModel
    {
        public int FromPartyId { get; set; }
        public string FromPartyName { get; set; }
        public int ToPartyId { get; set; }
        public string ToPartyName { get; set; }
        public int BillBranchId { get; set; }
        public string BillBranchName { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DropoffTime { get; set; }
        public IDictionary<int, int> FromPartyDic { get; set; }
        public IDictionary<int, int> ToPartyDic { get; set; }
        public IDictionary<int, int> BillBranchDic { get; set; }
        public IDictionary<int, DateTime?> PickupTimeDic { get; set; }
        public IDictionary<int, DateTime?> DropoffTimeDic { get; set; }
        public List<KeyHelper> Keys { get; set; }
        public List<int> FaultyKeys { get; set; }
    }
    public class KeyHelper
    {
        public int Id { get; set; }
    }
}
