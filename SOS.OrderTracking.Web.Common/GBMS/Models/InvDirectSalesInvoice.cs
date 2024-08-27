using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvDirectSalesInvoice
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
        public string XCustomer { get; set; } = null!;
        public string? XName { get; set; }
        public string? XNA { get; set; }
        public string? XNADescription { get; set; }
        public string? XContactPerson { get; set; }
        public string? XAddress { get; set; }
        public string? XLandlineNo { get; set; }
        public string? XMobileNo { get; set; }
        public string? XLoadingSupervisor { get; set; }
        public string? XSecurityRegisterNo { get; set; }
        public string? XVehicleType { get; set; }
        public string? XOwnedVehicleNo { get; set; }
        public string? XRentedVehicleNumber { get; set; }
        public string? XDriverName { get; set; }
        public decimal? XVehicleRent { get; set; }
        public decimal? XFreight { get; set; }
        public string? XBuiltyNumber { get; set; }
        public decimal? XSTaxPercent { get; set; }
        public decimal? XAddSTaxPercent { get; set; }
        public decimal? XAdvITaxPercent { get; set; }
        public decimal? XSedPercent { get; set; }
        public string? XUploadFromExcel { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
