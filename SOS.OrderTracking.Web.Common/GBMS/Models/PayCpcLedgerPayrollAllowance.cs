using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayCpcLedgerPayrollAllowance
    {
        public string YearCode { get; set; } = null!;
        public string MonthCode { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;
        public string? CardNumber { get; set; }
        public string CategoryCode { get; set; } = null!;
        public string LocationCode { get; set; } = null!;
        public string CadreCode { get; set; } = null!;
        public string DepartmentCode { get; set; } = null!;
        public string? PeCode { get; set; }
        public string? SectionCode { get; set; }
        public string DesignationCode { get; set; } = null!;
        public string GradeCode { get; set; } = null!;
        public string ShiftCode { get; set; } = null!;
        public string PayMode { get; set; } = null!;
        public string? BankCode { get; set; }
        public string? AccountType { get; set; }
        public string? AccountNumber { get; set; }
        public string SexCode { get; set; } = null!;
        public string ReligionCode { get; set; } = null!;
        public DateTime ProcessDate { get; set; }
        public string AlwCode { get; set; } = null!;
        public string AlwDesc { get; set; } = null!;
        public decimal AlwAmount { get; set; }
        public string? StatusCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? FatherName { get; set; }
        public string? XNtn { get; set; }
        public string? XCnic { get; set; }
        public string? XEobi { get; set; }
        public string? XSsNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoinDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public DateTime? ContractDate { get; set; }
        public string? YearDesc { get; set; }
        public string? MonthDesc { get; set; }
        public decimal? MastBas { get; set; }
        public decimal? MastGross { get; set; }
    }
}
