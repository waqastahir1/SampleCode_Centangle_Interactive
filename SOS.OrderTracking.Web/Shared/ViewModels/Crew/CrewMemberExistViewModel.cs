using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Crew
{
    public class CrewMemberExistViewModel
    {
        public int RelationshipId { get; set; }
        public int CrewId { get; set; }
        public string CrewName { get; set; }
        public string Employee { get; set; }
        public RoleType RoleType { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
