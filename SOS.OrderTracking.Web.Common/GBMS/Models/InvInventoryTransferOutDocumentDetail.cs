using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvInventoryTransferOutDocumentDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XCode { get; set; } = null!;
        public string? XDescription { get; set; }
        public string XUom { get; set; } = null!;
        public string? XNA { get; set; }
        public string? XNADescription { get; set; }
        public decimal XQuantity { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
