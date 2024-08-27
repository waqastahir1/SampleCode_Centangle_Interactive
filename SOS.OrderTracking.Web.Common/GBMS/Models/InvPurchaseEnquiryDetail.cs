using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvPurchaseEnquiryDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public decimal? XDemand { get; set; }
        public string? XDemandDate { get; set; }
        public string XItemCode { get; set; } = null!;
        public string? XDescription { get; set; }
        public string? XUom { get; set; }
        public decimal XQuantityRequired { get; set; }
        public string? XRequiredDate { get; set; }
        public string? XRemarks { get; set; }
        public string? XProject { get; set; }
        public string? XProjectDescription { get; set; }
        public string? XSpecifications { get; set; }
        public string? XInstructions { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
