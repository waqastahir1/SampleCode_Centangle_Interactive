using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class LmsLeaseScheduleRevisionAssetWiseLastVersion
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string? XPaymentDate { get; set; }
        public decimal? XPrinciple { get; set; }
        public decimal? XInterest { get; set; }
        public decimal? XInsurance { get; set; }
        public decimal? XTotalPayment { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
