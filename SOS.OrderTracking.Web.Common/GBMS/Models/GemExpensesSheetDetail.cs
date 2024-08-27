using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class GemExpensesSheetDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XExpenseCode { get; set; } = null!;
        public string? XExpenseCodeDescription { get; set; }
        public string? XDate { get; set; }
        public decimal? XAmountRequested { get; set; }
        public decimal XAmountApproved { get; set; }
        public string? XPurpose { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
