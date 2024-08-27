using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvPurchaseReturnNote
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
        public string XSupplier { get; set; } = null!;
        public string? XName { get; set; }
        public string XGrnNumber { get; set; } = null!;
        public string? XGrnDate { get; set; }
        public string XGrnDocumentType { get; set; } = null!;
        public string? XGrnDocumentTypeDescription { get; set; }
        public string? XDcNumber { get; set; }
        public string? XDcDate { get; set; }
        public string? XOgpNumber { get; set; }
        public string? XOgpDate { get; set; }
        public decimal? XFreight { get; set; }
        public decimal? XUnloading { get; set; }
        public decimal? XOthers { get; set; }
        public decimal? XSTaxPercent { get; set; }
        public decimal? XFTaxPercent { get; set; }
        public decimal? XETaxPercent { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
