using System;

namespace SOS.OrderTracking.Web.Common.GBMS.Models
{
    public partial class XShipmentAttachment
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; }
        public string WorkflowStatus { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public decimal XNumber { get; set; }
        public DateTime DDate { get; set; }
        public string XShipmentNo { get; set; }

        public string XLink { get; set; }

        public string XShipmentType { get; set; } = null!;
        public string? XShipmentTypeDescription { get; set; }
        public string XMainCustomer { get; set; } = null!;
        public string? XMainCustomerDescription { get; set; }
        public string XCollection { get; set; } = null!;
        public string? XCollectionDescription { get; set; }
        public string XDelivery { get; set; } = null!;
        public string? XDeliveryDescription { get; set; }
        public string? XBillBranch { get; set; }
        public string? XBillBranchDescription { get; set; }
        public string? XCustomer { get; set; }
        public string? XBillingCustomerName { get; set; }
        public string? XVehicle { get; set; }
        public string? XVehicleDescription { get; set; }
        public DateTime? DPickDate { get; set; }
        public DateTime? DPickTime { get; set; }
        public DateTime? DDeliveryDate { get; set; }
        public DateTime? DDeliveryTime { get; set; }
        public DateTime? DBillDate { get; set; }
        public decimal XNoOfBags { get; set; }
        public decimal XNoOfSeals { get; set; }
        public decimal XAmountCarried { get; set; }
        public string? XPortalReference { get; set; }
        public decimal? XVaultNights { get; set; }

    }
}
