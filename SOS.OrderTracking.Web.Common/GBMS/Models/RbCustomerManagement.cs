using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCustomerManagement
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XName { get; set; } = null!;
        public string? XAbbrevation { get; set; }
        public string? XCustomerType { get; set; }
        public string? XCustomerTypeDescription { get; set; }
        public string? XIndustryType { get; set; }
        public string? XIndustryTypeDescription { get; set; }
        public string? XRegistrationDate { get; set; }
        public DateTime? DRegistrationDate { get; set; }
        public string? XStatus { get; set; }
        public string? XStatusDescription { get; set; }
        public string? XMainCustomer { get; set; }
        public string? XMainCustomerDescription { get; set; }
        public string? XArea { get; set; }
        public string? XAreaDescription { get; set; }
        public string? XRevenueAuthority { get; set; }
        public string? XRevenueAuthorityDescription { get; set; }
        public string? XSalesTaxReg { get; set; }
        public string? XSalesTaxRegDescription { get; set; }
        public string? XSalesTaxNo { get; set; }
        public string? XSalesTaxExemption { get; set; }
        public string? XNtn { get; set; }
        public string? XCnic { get; set; }
        public string? XAddress1 { get; set; }
        public string? XAddress2 { get; set; }
        public string? XAddress3 { get; set; }
        public string? XLandline { get; set; }
        public string? XFaxNo { get; set; }
        public string? XCellNo { get; set; }
        public string? XEmailId { get; set; }
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
