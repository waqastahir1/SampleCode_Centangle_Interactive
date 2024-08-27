using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class SubRegionalHeadListViewModel
    {
        public int Id { get; set; }
        public int? RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }

        public string SubRegionName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int RelationshipId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}
