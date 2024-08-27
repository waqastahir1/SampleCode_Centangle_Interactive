using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbAtmServiceTypesContactPerson
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XName { get; set; }
        public string? XDesignation { get; set; }
        public string? XCnic { get; set; }
        public string? XCellNo { get; set; }
        public string? XLandline { get; set; }
        public string? XAddress { get; set; }
        public string? XEmailId { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
