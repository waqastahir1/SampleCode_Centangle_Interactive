namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class StrfedbnMl
    {
        public string Levcode { get; set; } = null!;
        public int Stmseqno { get; set; }
        public int Stiseqno { get; set; }
        public string Flcode { get; set; } = null!;
        public string Detcode { get; set; } = null!;

        public virtual Strfedbn StmseqnoNavigation { get; set; } = null!;
    }
}
