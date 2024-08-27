using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class GemExpensesSheet
    {
        public int MasterId { get; set; }
        public string DocumentStatus { get; set; } = null!;
        public string WorkflowStatus { get; set; } = null!;
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
        public decimal? XApprovedAmount { get; set; }
        public string? XDocumentStatus { get; set; }
        public string? XDocumentStatusDescription { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string? XDesignation { get; set; }
        public string? XDepartment { get; set; }
        public string? XExpenseDetails { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
