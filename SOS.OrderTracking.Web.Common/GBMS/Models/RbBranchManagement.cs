namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class RbBranchManagement
    {
        public string? BrCode { get; set; }
        public string BrName { get; set; } = null!;
        public string? SregCode { get; set; }
        public string? SregDesc { get; set; }
        public string? StatCode { get; set; }
        public string? StatDesc { get; set; }
        public string? CpcCode { get; set; }
        public string? CpcDesc { get; set; }
        public string? CpcYn { get; set; }
        public string McmCode { get; set; } = null!;
        public string McmName { get; set; } = null!;
        public string? BrType { get; set; }
        public string? BrTypeName { get; set; }
    }
}
