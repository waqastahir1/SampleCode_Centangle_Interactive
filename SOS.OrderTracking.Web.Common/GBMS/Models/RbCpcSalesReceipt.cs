using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCpcSalesReceipt
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
        public string XReceiptType { get; set; } = null!;
        public string? XReceiptTypeDescription { get; set; }
        public string? XManualReceiptNo { get; set; }
        public string XCustomer { get; set; } = null!;
        public string? XCustomerName { get; set; }
        public string XBank { get; set; } = null!;
        public string? XBankName { get; set; }
        public decimal? XWhiPercentage { get; set; }
        public decimal XAmount { get; set; }
        public decimal? XWhiAmount { get; set; }
        public decimal? XStwh { get; set; }
        public decimal? XOtherDed { get; set; }
        public decimal XNetReceived { get; set; }
        public string XInstrumentType { get; set; } = null!;
        public string? XInstrumentTypeDescription { get; set; }
        public string? XInstrumentNo { get; set; }
        public string? XInstDate { get; set; }
        public DateTime? DInstDate { get; set; }
        public string? XDrawnOnBank { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
