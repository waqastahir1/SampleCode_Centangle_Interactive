using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblSupplierInvoicePoAdvance
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XPoNumber { get; set; } = null!;
        public string? XPoDate { get; set; }
        public DateTime DPoDate { get; set; }
        public string XPoLocation { get; set; } = null!;
        public string? XPoLocationDescription { get; set; }
        public decimal XPoAmount { get; set; }
        public decimal XAdvance { get; set; }
        public decimal? XAlreadyAdjusted { get; set; }
        public decimal? XAdjustedNow { get; set; }
        public decimal? XBalance { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
