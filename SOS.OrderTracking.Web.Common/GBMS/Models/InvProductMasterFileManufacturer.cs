using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvProductMasterFileManufacturer
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XManufacturer { get; set; }
        public string? XManufacturerDescription { get; set; }
        public string? XBrandName { get; set; }
        public string? XUom { get; set; }
        public string? XUomDescription { get; set; }
        public string? XCountry { get; set; }
        public string? XCountryDescription { get; set; }
        public decimal? XPackingSize { get; set; }
        public decimal? XPackingSize2 { get; set; }
        public decimal? XPackingSize3 { get; set; }
        public decimal? XPackingSize4 { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
