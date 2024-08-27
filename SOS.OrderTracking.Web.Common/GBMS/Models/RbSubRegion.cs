﻿using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbSubRegion
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XDescription { get; set; } = null!;
        public string? XAbbrevation { get; set; }
        public string XRegion { get; set; } = null!;
        public string? XRegionDescription { get; set; }
        public string? XRevenueAuthroity { get; set; }
        public string? XRevenueAuthroityDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
