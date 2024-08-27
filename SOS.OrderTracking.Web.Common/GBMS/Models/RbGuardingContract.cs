using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbGuardingContract
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string XName { get; set; } = null!;
        public string? XStartDate { get; set; }
        public DateTime DStartDate { get; set; }
        public string? XEndDate { get; set; }
        public DateTime? DEndDate { get; set; }
        public string? XContractStatus { get; set; }
        public string? XContractStatusDescription { get; set; }
        public string XMainCustomer { get; set; } = null!;
        public string? XMainCustomerDescription { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
