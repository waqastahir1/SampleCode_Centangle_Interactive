using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingContractBranch
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string XBranchCode { get; set; } = null!;
        public string? XBranchCodeDescription { get; set; }
        public string? XStartDate { get; set; }
        public DateTime? DStartDate { get; set; }
        public string? XEndDate { get; set; }
        public DateTime? DEndDate { get; set; }
        public string? XStatus { get; set; }
        public string? XStatusDescription { get; set; }
        public string XServiceType { get; set; } = null!;
        public string? XServiceTypeDescription { get; set; }
        public string XServiceCode { get; set; } = null!;
        public string? XServiceCodeDescription { get; set; }
        public decimal XNoOfGuards { get; set; }
        public decimal? XMorning { get; set; }
        public decimal? XEvening { get; set; }
        public string XBranchLevelInvoiceRates { get; set; } = null!;
        public decimal XInvoiceRate { get; set; }
        public decimal XInvoiceAmount { get; set; }
        public string? XBranchLevelSalaryRates { get; set; }
        public decimal? XSalaryRate { get; set; }
        public decimal? XSalaryAmount { get; set; }
        public decimal? XServiceRate { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
