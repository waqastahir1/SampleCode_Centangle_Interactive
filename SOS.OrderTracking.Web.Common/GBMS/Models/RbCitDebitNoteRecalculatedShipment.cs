using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbCitDebitNoteRecalculatedShipment
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
        public string XRevenueAuthority { get; set; } = null!;
        public string? XRevenueAuthorityDescription { get; set; }
        public string XCustomer { get; set; } = null!;
        public string? XName { get; set; }
        public decimal XSTaxPercent { get; set; }
        public decimal? XInvoiceNumber { get; set; }
        public string? XMainCode { get; set; }
        public string? XMainCodeDescription { get; set; }
        public string? XStation { get; set; }
        public string? XStationDescription { get; set; }
        public decimal? XAmount { get; set; }
        public decimal? XSalesTax { get; set; }
        public decimal? XTotalAmount { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
