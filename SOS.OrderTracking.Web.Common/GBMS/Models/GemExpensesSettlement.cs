﻿using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class GemExpensesSettlement
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
        public string XAdvanceNumber { get; set; } = null!;
        public string? XAdvanceNumberDescription { get; set; }
        public string XStation { get; set; } = null!;
        public string? XStationDescription { get; set; }
        public string XEmployee { get; set; } = null!;
        public string? XEmployeeDescription { get; set; }
        public string? XDesignation { get; set; }
        public string? XDepartment { get; set; }
        public string XExpenseDetails { get; set; } = null!;
        public decimal? XTotalAmount { get; set; }
        public decimal XActualAmount { get; set; }
        public decimal? XBalanceAmount { get; set; }
        public string? XAccount { get; set; }
        public string? XBankOrCashAccountDescription { get; set; }
        public string? XInstrumentType { get; set; }
        public string? XInstrumentTypeDescription { get; set; }
        public string? XInstrumentNo { get; set; }
        public string? XInstDate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
