using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcOtherVendorShipment
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
        public string? UserId { get; set; }
        public string YearCode { get; set; } = null!;
        public string? YearName { get; set; }
        public string PeriodCode { get; set; } = null!;
        public string? PeriodName { get; set; }
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public decimal XNumber { get; set; }
        public string? XDate { get; set; }
        public DateTime DDate { get; set; }
        public string XShipmentNo { get; set; } = null!;
        public string XSecurityCompany { get; set; } = null!;
        public string? XSecurityCompanyDescription { get; set; }
        public string XMainCustomer { get; set; } = null!;
        public string? XMainCustomerDescription { get; set; }
        public string? XCollection { get; set; }
        public string? XCollectionDescription { get; set; }
        public string? XDelivery { get; set; }
        public string? XDeliveryDescription { get; set; }
        public string? XBillBranch { get; set; }
        public string? XBillBranchDescription { get; set; }
        public string? XCustomer { get; set; }
        public string? XBillingCustomerName { get; set; }
        public string? XPickDate { get; set; }
        public DateTime DPickDate { get; set; }
        public string? XPickTime { get; set; }
        public DateTime? DPickTime { get; set; }
        public string? XDeliveryDate { get; set; }
        public DateTime? DDeliveryDate { get; set; }
        public string? XDeliveryTime { get; set; }
        public DateTime? DDeliveryTime { get; set; }
        public decimal XNoOfBags { get; set; }
        public decimal XNoOfSeals { get; set; }
        public decimal XAmountCarried { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
