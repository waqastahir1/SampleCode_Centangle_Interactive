using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvProductClassificationSalesLink
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XDirectSales { get; set; }
        public string? XDirectSalesDescription { get; set; }
        public string? XSalesAccount { get; set; }
        public string? XSalesAccountDescription { get; set; }
        public string? XDepotSales { get; set; }
        public string? XDepotSalesDescription { get; set; }
        public string? XFactorySales { get; set; }
        public string? XFactorySalesDescription { get; set; }
        public string? XSalesTax { get; set; }
        public string? XSalesTaxDescription { get; set; }
        public string? XExciseDuty { get; set; }
        public string? XExciseDutyDescription { get; set; }
        public string? XTradeDiscount { get; set; }
        public string? XTradeDiscountDescription { get; set; }
        public string? XMarketingSamples { get; set; }
        public string? XMarketingSamplesDescription { get; set; }
        public string? XComplementary { get; set; }
        public string? XComplementaryDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
