using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultConsignmentListViewModel
    {
        public int DeliveryId { get; set; }

        public int? PreviousId { get; set; }
        public int? VaultId { get; set; }
        public string ShipmentCode { get; set; }
        public ConsignmentDeliveryState ConsignmentStateType { get; set; }
        public string CrewName { get; set; }
        public string FromPartyCode { get; set; }
        public string ToPartyCode { get; set; }
        public int Amount { get; set; }
        public int BillBranchId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
