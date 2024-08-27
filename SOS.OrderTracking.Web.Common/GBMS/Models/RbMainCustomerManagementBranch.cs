using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbMainCustomerManagementBranch
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XBranchCode { get; set; } = null!;
        public string XBranchName { get; set; } = null!;
        public string? XBranchType { get; set; }
        public string? XBranchTypeDescription { get; set; }
        public string? XCpcBranch { get; set; }
        public string? XSubRegion { get; set; }
        public string? XSubRegionDescription { get; set; }
        public string? XStation { get; set; }
        public string? XStationDescription { get; set; }
        public string? XCpc { get; set; }
        public string? XCpcDescription { get; set; }
        public string? XAtmBranch { get; set; }
        public string? XAtmBranchDescription { get; set; }
        public string? XAlramSystem { get; set; }
        public string? XAlramSystemDescription { get; set; }
        public string? XBranchStatus { get; set; }
        public string? XBranchStatusDescription { get; set; }
        public string? XAtmCitBill { get; set; }
        public string? XAtmCitBillDescription { get; set; }
        public string? XGeoStatus { get; set; }
        public string? XGeoStatusDescription { get; set; }
        public string? XLatitude { get; set; }
        public string? XLongitude { get; set; }
        public string? XAddress1 { get; set; }
        public string? XAddress2 { get; set; }
        public string? XLandline { get; set; }
        public string? XFaxNumber { get; set; }
        public string? XMobile { get; set; }
        public string? XEmail { get; set; }
        public string? XContactPersonDetails { get; set; }
        public string? XGuardingStatus { get; set; }
        public string? XGuardingStatusDescription { get; set; }
        public string? XEndDate { get; set; }
        public DateTime? DEndDate { get; set; }
        public string? XGuardingRegion { get; set; }
        public string? XGuardingRegionDescription { get; set; }
        public string? XGuardingStation { get; set; }
        public string? XGuardingStationDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? XBranchDetailFeatures { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
