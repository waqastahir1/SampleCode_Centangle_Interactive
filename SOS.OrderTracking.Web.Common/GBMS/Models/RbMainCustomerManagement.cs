using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbMainCustomerManagement
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XName { get; set; } = null!;
        public string? XAbbrevation { get; set; }
        public string? XCitCentralized { get; set; }
        public decimal? XLocalRangeInKms { get; set; }
        public string? XShipmentDate { get; set; }
        public string? XShipmentDateDescription { get; set; }
        public string? XCitInvoicing { get; set; }
        public string? XCitInvoicingDescription { get; set; }
        public string? XGuardingCentralized { get; set; }
        public string? XCpcCentralized { get; set; }
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
        public string? XBankName { get; set; }
        public string? XAccountTitle { get; set; }
        public string? XAccountNumber { get; set; }
        public string? XIbanNumber { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
