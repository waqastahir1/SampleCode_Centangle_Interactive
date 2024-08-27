using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class LmsLeaseScheduleAssetWise
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
        public string XAgreement { get; set; } = null!;
        public string? XAgreementDescription { get; set; }
        public string XLeaseType { get; set; } = null!;
        public string? XLeaseTypeDescription { get; set; }
        public string XSupplier { get; set; } = null!;
        public string? XName { get; set; }
        public string XAsset { get; set; } = null!;
        public string? XAssetDescription { get; set; }
        public decimal XPurchasePrice { get; set; }
        public decimal XDpPercent { get; set; }
        public decimal XDownPayment { get; set; }
        public decimal XLoanAmount { get; set; }
        public decimal XTenureYears { get; set; }
        public decimal XInstallments { get; set; }
        public decimal XMarkupRate { get; set; }
        public string? XStartDate { get; set; }
        public decimal? XInsRate { get; set; }
        public decimal? XInsurance { get; set; }
        public decimal? XProcessingFee { get; set; }
        public decimal? XRegisteration { get; set; }
        public string? XBankCode { get; set; }
        public string? XBankCodeDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? XUploadFromExcel { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
