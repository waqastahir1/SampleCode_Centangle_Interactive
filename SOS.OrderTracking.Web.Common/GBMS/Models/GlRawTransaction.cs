using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class GlRawTransaction
    {
        public int MasterId { get; set; }
        public string LocationCode { get; set; } = null!;
        public string DocumentType { get; set; } = null!;
        public decimal DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public string? XPaidTo { get; set; }
        public string Xstatus { get; set; } = null!;
        public string? XInstrument { get; set; }
        public string? InstrumentNo { get; set; }
        public DateTime? InstrumentDate { get; set; }
        public string? MasterPart { get; set; }
        public decimal DetailId { get; set; }
        public string? DetailPart { get; set; }
        public string AccountCode { get; set; } = null!;
        public string DC { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? CustomerCode { get; set; }
        public string? SupplierCode { get; set; }
        public string? EmployeeCode { get; set; }
        public string? DepartmentCode { get; set; }
        public string? AssetCode { get; set; }
        public string? ProjectCode { get; set; }
        public string LocationDesc { get; set; } = null!;
        public string DocumentDesc { get; set; } = null!;
    }
}
