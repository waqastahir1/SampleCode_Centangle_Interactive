using System;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class Strfedbn
    {
        public Strfedbn()
        {
            StrfedbnD1s = new HashSet<StrfedbnD1>();
        }

        public int Stmseqno { get; set; }
        public string Yrcode { get; set; } = null!;
        public string Prcode { get; set; } = null!;
        public string Lccode { get; set; } = null!;
        public decimal Stmdocno { get; set; }
        public DateTime Stmdocdate { get; set; }
        public string Stmstatus { get; set; } = null!;
        public string Wfstatus { get; set; } = null!;
        public string Dtcode { get; set; } = null!;
        public decimal Dtcardtype { get; set; }
        public string Spcode { get; set; } = null!;
        public string? Accode { get; set; }
        public decimal? Stmdiscp { get; set; }
        public decimal? Stmdisc { get; set; }
        public decimal? Stmstaxp { get; set; }
        public decimal? Stmstax { get; set; }
        public decimal? Stmitaxp { get; set; }
        public decimal? Stmitax { get; set; }
        public decimal? Stmsedp { get; set; }
        public decimal? Stmsed { get; set; }
        public decimal? Stmfreight { get; set; }
        public decimal? Stmoctroi { get; set; }
        public decimal? Stmothr { get; set; }
        public decimal? Stmnetvalue { get; set; }
        public string? Stmfl1 { get; set; }
        public string? Stmfl2 { get; set; }
        public string? Stmfl3 { get; set; }
        public string? Stmfl4 { get; set; }
        public string? Stmfl5 { get; set; }
        public string? Stmfl6 { get; set; }
        public string? Stmfl7 { get; set; }
        public string? Stmfl8 { get; set; }
        public string? Stmfl9 { get; set; }
        public string? Stmfl10 { get; set; }
        public string? Stmfl11 { get; set; }
        public string? Stmfl12 { get; set; }
        public string? Stmfl13 { get; set; }
        public string? Stmfl14 { get; set; }
        public string? Stmfl15 { get; set; }
        public string? Stmfl16 { get; set; }
        public string? Stmfl17 { get; set; }
        public string? Stmfl18 { get; set; }
        public string? Stmfl19 { get; set; }
        public string? Stmfl20 { get; set; }
        public string? Stmfl21 { get; set; }
        public string? Stmfl22 { get; set; }
        public string? Stmfl23 { get; set; }
        public string? Stmfl24 { get; set; }
        public string? Stmfl25 { get; set; }
        public string? Stmfl26 { get; set; }
        public string? Stmfl27 { get; set; }
        public string? Stmfl28 { get; set; }
        public string? Stmfl29 { get; set; }
        public string? Stmfl30 { get; set; }
        public string? Stmfl31 { get; set; }
        public string? Stmfl32 { get; set; }
        public string? Stmfl33 { get; set; }
        public string? Stmfl34 { get; set; }
        public string? Stmfl35 { get; set; }
        public string? Stmfl36 { get; set; }
        public string? Stmfl37 { get; set; }
        public string? Stmfl38 { get; set; }
        public string? Stmfl39 { get; set; }
        public string? Stmfl40 { get; set; }
        public string? Stmfl41 { get; set; }
        public string? Stmfl42 { get; set; }
        public string? Stmfl43 { get; set; }
        public string? Stmfl44 { get; set; }
        public string? Stmfl45 { get; set; }
        public string? Stmfl46 { get; set; }
        public string? Stmfl47 { get; set; }
        public string? Stmfl48 { get; set; }
        public string? Stmfl49 { get; set; }
        public string? Stmfl50 { get; set; }
        public decimal? Stmfln1 { get; set; }
        public decimal? Stmfln2 { get; set; }
        public decimal? Stmfln3 { get; set; }
        public decimal? Stmfln4 { get; set; }
        public decimal? Stmfln5 { get; set; }
        public decimal? Stmfln6 { get; set; }
        public decimal? Stmfln7 { get; set; }
        public decimal? Stmfln8 { get; set; }
        public decimal? Stmfln9 { get; set; }
        public decimal? Stmfln10 { get; set; }
        public decimal? Stmfln11 { get; set; }
        public decimal? Stmfln12 { get; set; }
        public decimal? Stmfln13 { get; set; }
        public decimal? Stmfln14 { get; set; }
        public decimal? Stmfln15 { get; set; }
        public decimal? Stmfln16 { get; set; }
        public decimal? Stmfln17 { get; set; }
        public decimal? Stmfln18 { get; set; }
        public decimal? Stmfln19 { get; set; }
        public decimal? Stmfln20 { get; set; }
        public decimal? Stmfln21 { get; set; }
        public decimal? Stmfln22 { get; set; }
        public decimal? Stmfln23 { get; set; }
        public decimal? Stmfln24 { get; set; }
        public decimal? Stmfln25 { get; set; }
        public DateTime? Stmfld1 { get; set; }
        public DateTime? Stmfld2 { get; set; }
        public DateTime? Stmfld3 { get; set; }
        public DateTime? Stmfld4 { get; set; }
        public DateTime? Stmfld5 { get; set; }
        public DateTime? Stmfld6 { get; set; }
        public DateTime? Stmfld7 { get; set; }
        public DateTime? Stmfld8 { get; set; }
        public DateTime? Stmfld9 { get; set; }
        public DateTime? Stmfld10 { get; set; }
        public DateTime? Stmfld11 { get; set; }
        public DateTime? Stmfld12 { get; set; }
        public DateTime? Stmfld13 { get; set; }
        public DateTime? Stmfld14 { get; set; }
        public DateTime? Stmfld15 { get; set; }
        public string Stmcd1 { get; set; } = null!;
        public string? Stmcd2 { get; set; }
        public string? Stmcd3 { get; set; }
        public string? Stmcd4 { get; set; }
        public string? Stmcd5 { get; set; }
        public string? Stmcd6 { get; set; }
        public string? Stmcd7 { get; set; }
        public string? Stmcd8 { get; set; }
        public string? Stmcd9 { get; set; }
        public string? Stmcd10 { get; set; }
        public string? Stmcd11 { get; set; }
        public string? Stmcd12 { get; set; }
        public string? Stmcd13 { get; set; }
        public string? Stmcd14 { get; set; }
        public string? Stmcd15 { get; set; }
        public string? Stmcd16 { get; set; }
        public string? Stmcd17 { get; set; }
        public string? Stmcd18 { get; set; }
        public string? Stmcd19 { get; set; }
        public string? Stmcd20 { get; set; }
        public string? Stmcd21 { get; set; }
        public string? Stmcd22 { get; set; }
        public string? Stmcd23 { get; set; }
        public string? Stmcd24 { get; set; }
        public string? Stmcd25 { get; set; }
        public string? UseridAdd { get; set; }
        public string? UseridModify { get; set; }
        public string? IpAdd { get; set; }
        public string? SysAdd { get; set; }
        public DateTime? AddDate { get; set; }
        public string? IpMod { get; set; }
        public string? SysMod { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IdDel { get; set; }
        public string? IpDel { get; set; }
        public string? SysDel { get; set; }
        public DateTime? DelDate { get; set; }

        public virtual ICollection<StrfedbnD1> StrfedbnD1s { get; set; }
    }
}
