using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayGuardingPayrollDisbursement
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
        public DateTime DDate { get; set; }
        public string? XSalaryDate { get; set; }
        public DateTime? DSalaryDate { get; set; }
        public decimal? XDisbursedAmount { get; set; }
        public string? XDisbursementSelection { get; set; }
        public string? XBankName { get; set; }
        public string? XBankNameDescription { get; set; }
        public decimal? XMaximumSalary { get; set; }
        public string? XBank { get; set; }
        public string? XBankAccountName { get; set; }
        public string? XInstrumentType { get; set; }
        public string? XInstrumentTypeDescription { get; set; }
        public string? XInstrumentNo { get; set; }
        public string? XInstDate { get; set; }
        public DateTime? DInstDate { get; set; }
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
