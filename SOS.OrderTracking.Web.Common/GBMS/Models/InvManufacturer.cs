using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvManufacturer
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XName { get; set; } = null!;
        public string? XAbbrevation { get; set; }
        public string XType { get; set; } = null!;
        public string? XTypeDescription { get; set; }
        public string XStatus { get; set; } = null!;
        public string? XStatusDescription { get; set; }
        public string? XSalesTaxNo { get; set; }
        public string? XNtn { get; set; }
        public string? XCnic { get; set; }
        public string? XAddress1 { get; set; }
        public string? XAddress2 { get; set; }
        public string? XAddress3 { get; set; }
        public string? XLandline { get; set; }
        public string? XMobile { get; set; }
        public string? XFaxNo { get; set; }
        public string? XEmail { get; set; }
        public string? XUrl { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
