using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCitShipmentCopy03
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
        public string? UserId { get; set; }
        public string YearCode { get; set; } = null!;
        public string? YearName { get; set; }
        public string PeriodCode { get; set; } = null!;
        public string? PeriodName { get; set; }
        public int MonthOrder { get; set; }
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public decimal XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public string XShipmentNo { get; set; } = null!;
        public string XShipmentType { get; set; } = null!;
        public string? XShipmentTypeDescription { get; set; }
        public string XMainCustomer { get; set; } = null!;
        public string? XMainCustomerDescription { get; set; }
        public string XCollection { get; set; } = null!;
        public string? CollectionClient { get; set; }
        public string CollectionClientDescription { get; set; } = null!;
        public string? XCollectionDescription { get; set; }
        public string XDelivery { get; set; } = null!;
        public string? DeliveryClient { get; set; }
        public string DeliveryClientDescription { get; set; } = null!;
        public string? XDeliveryDescription { get; set; }
        public string? XBillBranch { get; set; }
        public string? XBillBranchDescription { get; set; }
        public string? XCustomer { get; set; }
        public string? XBillingCustomerName { get; set; }
        public string? XRevenueRegion { get; set; }
        public string? XRevenueRegionDescription { get; set; }
        public string? XVehicle { get; set; }
        public string? XVehicleDescription { get; set; }
        public string? XPickDate { get; set; }
        public DateTime? DPickDate { get; set; }
        public string? XPickTime { get; set; }
        public DateTime? DPickTime { get; set; }
        public string? XDeliveryDate { get; set; }
        public DateTime? DDeliveryDate { get; set; }
        public string? XDeliveryTime { get; set; }
        public DateTime? DDeliveryTime { get; set; }
        public decimal XNoOfBags { get; set; }
        public decimal XNoOfSeals { get; set; }
        public decimal XAmountCarried { get; set; }
        public decimal XDistanceKms { get; set; }
        public decimal? XTollTax { get; set; }
        public decimal? XWaitingMins { get; set; }
        public decimal? XAdditional { get; set; }
        public decimal? XVaultNights { get; set; }
        public string XAutoBilling { get; set; } = null!;
        public string? XRateId { get; set; }
        public string? XRateIdDescription { get; set; }
        public decimal? XFixedAmount { get; set; }
        public string? XFcValue { get; set; }
        public decimal XBaseCharges { get; set; }
        public decimal XSurCharge { get; set; }
        public decimal XSealCharges { get; set; }
        public decimal XVaultCharges { get; set; }
        public decimal? XDistance { get; set; }
        public decimal? XWaiting { get; set; }
        public decimal? XAmount { get; set; }
        public decimal? XDebits { get; set; }
        public decimal? XCredits { get; set; }
        public decimal XNetAmount { get; set; }
        public string? XPortalReference { get; set; }
        public string? XBillDate { get; set; }
        public DateTime? DBillDate { get; set; }
        public string? XCrnReversed { get; set; }
        public string? XCrnReversedDescription { get; set; }
        public string? XStationDescription { get; set; }
        public string? XVehicleCodeDescription { get; set; }
    }
}
