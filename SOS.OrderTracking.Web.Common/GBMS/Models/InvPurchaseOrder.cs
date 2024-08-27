using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvPurchaseOrder
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
        public string? UserId { get; set; }
        public string YearCode { get; set; } = null!;
        public string? YearName { get; set; }
        public string PeriodCode { get; set; } = null!;
        public string? PeriodName { get; set; }
        public string LocationCode { get; set; } = null!;
        public string? LocationName { get; set; }
        public decimal XNumber { get; set; }
        public string? XDate { get; set; }
        public string? XPoSourceDocument { get; set; }
        public string? XSourceDoc { get; set; }
        public string? XSourceDocDt { get; set; }
        public string? XDepartment { get; set; }
        public string? XDepartmentDescription { get; set; }
        public string XPoType { get; set; } = null!;
        public string? XPoTypeDescription { get; set; }
        public string XSupplier { get; set; } = null!;
        public string? XSupplierName { get; set; }
        public string? XContactPersonDetails { get; set; }
        public string XPartialShipment { get; set; } = null!;
        public string? XPartialShipmentDescription { get; set; }
        public string? XDeliveryTerms { get; set; }
        public string? XCreditPeriod { get; set; }
        public string? XModeOfPayment { get; set; }
        public string? XDueDate { get; set; }
        public string? XFreightPaidBy { get; set; }
        public string? XCurrency { get; set; }
        public string? XCurrencyDescription { get; set; }
        public string? XSupplierRef { get; set; }
        public string? XRefDate { get; set; }
        public string? XPaymentTermsTemplate { get; set; }
        public string? XPaymentTermsTemplateDescription { get; set; }
        public decimal? XSTaxPercent { get; set; }
        public decimal? XFTaxPercent { get; set; }
        public decimal? XETaxPercent { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
