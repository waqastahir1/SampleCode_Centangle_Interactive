using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class RegionalHeadListViewModel
    {
        public int Id { get; set; }
        public int? RegionId { get; set; }

        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
        public string RegionName { get; set; }
    }
}
