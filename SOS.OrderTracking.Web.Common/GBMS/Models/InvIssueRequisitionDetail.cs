using System;

namespace SOS.OrderTracking.Web.Portal.GBMS.Models
{
    public partial class InvIssueRequisitionDetail
    {
        public int MasterId { get; set; }
        public int DetailId { get; set; }
        public string XItemCode { get; set; } = null!;
        public string? XDescription { get; set; }
        public string? XUom { get; set; }
        public decimal? XQuantityDemanded { get; set; }
        public decimal XQuantityIssued { get; set; }
        public string? XProject { get; set; }
        public string? XProjectDescription { get; set; }
        public string? XCostCenter { get; set; }
        public string? XCostCenterDescription { get; set; }
        public string? XRegion { get; set; }
        public string? XRegionDescription { get; set; }
        public string? XDepartment { get; set; }
        public string? XDepartmentDescription { get; set; }
        public string? XEmployee { get; set; }
        public string? XEmployeeDescription { get; set; }
        public string? XVehicle { get; set; }
        public string? XVehicleDescription { get; set; }
        public string? XBranch { get; set; }
        public string? XBranchDescription { get; set; }
        public string? XChargeableCode { get; set; }
        public string? XChargeableCodeName { get; set; }
        public decimal? XOdometer { get; set; }
        public string? XRemarks { get; set; }
        public string? AddId { get; set; }
        public string? ModId { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string? IpAdd { get; set; }
        public string? IpMod { get; set; }
    }
}
