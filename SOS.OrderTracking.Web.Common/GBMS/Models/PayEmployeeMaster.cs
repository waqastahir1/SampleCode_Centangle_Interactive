using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class PayEmployeeMaster
    {
        public string XCode { get; set; } = null!;
        public long? XrowId { get; set; }
        public string? XName { get; set; }
        public string? XStatus { get; set; }
        public string? XGender { get; set; }
        public string? XFatherName { get; set; }
        public string? XHusbandName { get; set; }
        public decimal? XGrossPay { get; set; }
        public decimal? XBasicPay { get; set; }
        public decimal? XAdvLimit { get; set; }
        public string? XShift { get; set; }
        public string? XShiftDescription { get; set; }
        public string? XReligion { get; set; }
        public string? XReligionDescription { get; set; }
        public string? XDateOfBirth { get; set; }
        public string? XJoiningDate { get; set; }
        public string? XContractDate { get; set; }
        public string? XExpiryDate { get; set; }
        public string? XCard { get; set; }
        public string? XCnic { get; set; }
        public string? XNtn { get; set; }
        public string? XEobi { get; set; }
        public string? XSocialSecurity { get; set; }
        public string? XDesignation { get; set; }
        public string? XDesignationDescription { get; set; }
        public string? XCadre { get; set; }
        public string? XCadreDescription { get; set; }
        public string? XGrade { get; set; }
        public string? XGradeDescription { get; set; }
        public string? XCategory { get; set; }
        public string? XCategoryDescription { get; set; }
        public string? XLocation { get; set; }
        public string? XLocationDescription { get; set; }
        public string? XDepartment { get; set; }
        public string? XDepartmentDescription { get; set; }
        public string? XSection { get; set; }
        public string? XSectionDescription { get; set; }
        public string? XProject { get; set; }
        public string? XProjectDescription { get; set; }
        public string? XPayMode { get; set; }
        public string? XBankName { get; set; }
        public string? XBankNameDescription { get; set; }
        public string? XAccountNumber { get; set; }
        public string? XAddress { get; set; }
        public string? XCity { get; set; }
        public string? XPersonalMobile { get; set; }
        public string? XOfficialMobile { get; set; }
        public string? XPersonalEmail { get; set; }
        public string? XOfficialEmail { get; set; }
        public string? XReference1 { get; set; }
        public string? XReference2 { get; set; }
        public string? XBloodGroup { get; set; }
        public decimal? XNa1 { get; set; }
        public decimal? XNa2 { get; set; }
        public decimal? XNa3 { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
