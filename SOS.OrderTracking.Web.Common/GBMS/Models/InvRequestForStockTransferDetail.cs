using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvRequestForStockTransferDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XItemCode { get; set; } = null!;
        public string? XDescription { get; set; }
        public string? XUom { get; set; }
        public string? XProject { get; set; }
        public string? XProjectDescription { get; set; }
        public decimal XQuantity { get; set; }
        public string? XVehicleNo { get; set; }
        public string? XReferenceNo { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
