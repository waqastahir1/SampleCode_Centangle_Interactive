﻿using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingSalesTaxInvoiceBranchDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XBranchCode { get; set; } = null!;
        public string? XBranchCodeDescription { get; set; }
        public string XServiceType { get; set; } = null!;
        public string? XServiceTypeDescription { get; set; }
        public string XRevenueCode { get; set; } = null!;
        public string? XRevenueCodeDescription { get; set; }
        public decimal XNoOfGuards { get; set; }
        public decimal? XTotalDays { get; set; }
        public decimal? XReducedDays { get; set; }
        public decimal? XChargedDays { get; set; }
        public decimal? XMonthlyRate { get; set; }
        public decimal XDailyRate { get; set; }
        public decimal XTotalValue { get; set; }
        public decimal? XServiceRate { get; set; }
        public decimal? XServiceAmount { get; set; }
        public decimal? XSalesTax { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
