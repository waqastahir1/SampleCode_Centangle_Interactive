using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class ShipmentVaultReportListViewModel
    {
        public int Id { get; set; }
        public string Client { get; set; }
        public string ShipmentCode { get; set; }
        public string PickupBranch { get; set; }
        public string DeliveryBranch { get; set; }
        public string PickupTime { get; set; }
        public string DeliveryTime { get; set; }
        public string CollectionVehicle { get; set; }
        public string DropoffVehicle { get; set; }
        public string VehicleIn { get; set; }
        public string VehicleOut { get; set; }
        public string VaultInCrew { get; set; }
        public string VaultOutCrew { get; set; }
        public string VaultInTime { get; set; }
        public string VaultOutTime { get; set; }
        public int Amount { get; set; }
        public string AmountStr { get; set; }
        public string ShipmentScan { get; set; }
        public string Station { get; set; }
        public int SealCount { get; set; }
        public int BagsCount { get; set; }
        public ConsignmentDeliveryModel CollectionDelivery { get; set; }
        public ConsignmentDeliveryModel DroppoffDelivery { get; set; }
        public string CurrentDate { get; set; }
        public ConsignmentStatus ConsignmentStatus { get; set; }
        public ConsignmentDeliveryState ConsignmentState { get; set; }
        public string VaultType { get; set; }
    }
}
