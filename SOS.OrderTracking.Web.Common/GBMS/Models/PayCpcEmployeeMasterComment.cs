using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCpcEmployeeMasterComment
    {
        public string XCode { get; set; } = null!;
        public int DetailId { get; set; }
        public string? XCommentType { get; set; }
        public string? XCommentTypeDescription { get; set; }
        public string? XDate { get; set; }
        public DateTime? DDate { get; set; }
        public string? XOccasionForComment { get; set; }
        public string? XRemarks1 { get; set; }
        public string? XRemarks2 { get; set; }
        public string? XRemarks3 { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
