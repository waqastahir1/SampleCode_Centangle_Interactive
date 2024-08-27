using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class GemExpensesAdvance
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
        public string XStation { get; set; } = null!;
        public string? XStationDescription { get; set; }
        public decimal XAdvanceAmount { get; set; }
        public string? XDocumentStatus { get; set; }
        public string? XDocumentStatusDescription { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string? XDesignation { get; set; }
        public string? XDepartment { get; set; }
        public string XAdvanceDetails { get; set; } = null!;
        public string XAdvanceType { get; set; } = null!;
        public string? XAdvanceTypeDescription { get; set; }
        public string? XAccountTitle { get; set; }
        public string? XBankAccountNumber { get; set; }
        public string? XBankName { get; set; }
        public string? XBankNameDescription { get; set; }
        public string? XBeneficiaryAccountTitle { get; set; }
        public string? XBeneficiaryAccountNumber { get; set; }
        public string? XBeneficiaryMobileNumber { get; set; }
        public string? XBeneficiaryBankName { get; set; }
        public string? XBeneficiaryBankNameDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
