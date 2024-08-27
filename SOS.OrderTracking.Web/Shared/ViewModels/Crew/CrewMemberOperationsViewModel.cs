using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewMemberOperationsViewModel
    {

        public int RelationshipId { get; set; }

        public int CrewId { get; set; }

        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
