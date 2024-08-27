﻿using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PdwGradesAllowance
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XAllowanceSelection { get; set; } = null!;
        public string? XAllowanceSelectionDescription { get; set; }
        public string XApplyToggle { get; set; } = null!;
        public string? XToggleSelection { get; set; }
        public string? XToggleSelectionDescription { get; set; }
        public decimal? XAmount { get; set; }
        public string XAttFactor { get; set; } = null!;
        public string XApplyDates { get; set; } = null!;
        public string? XStartingDate { get; set; }
        public string? XEndingDate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
