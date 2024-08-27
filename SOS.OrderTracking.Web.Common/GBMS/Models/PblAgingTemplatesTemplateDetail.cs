﻿using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PblAgingTemplatesTemplateDetail
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XAgingPeriodTitle { get; set; } = null!;
        public decimal XFromDays { get; set; }
        public decimal XToDays { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
