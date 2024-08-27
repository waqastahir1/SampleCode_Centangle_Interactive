using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmSalesTaxInvoiceAtmServiceTripDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XTripNumber { get; set; }
        public string XTripType { get; set; } = null!;
        public string? XTripTypeDescription { get; set; }
        public decimal? XTripCharges { get; set; }
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
