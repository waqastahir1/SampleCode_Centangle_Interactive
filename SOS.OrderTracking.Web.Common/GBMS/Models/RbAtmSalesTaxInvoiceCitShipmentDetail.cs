using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmSalesTaxInvoiceCitShipmentDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XShipmentNumber { get; set; }
        public string XShipmentType { get; set; } = null!;
        public string? XShipmentTypeDescription { get; set; }
        public decimal? XBaseCharges { get; set; }
        public decimal? XSurCharge { get; set; }
        public decimal? XSealCharges { get; set; }
        public decimal? XVaultCharges { get; set; }
        public decimal? XDistance { get; set; }
        public decimal? XOtherCharges { get; set; }
        public decimal? XNetValue { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
