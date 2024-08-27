using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Common.ViewModels.Crew
{
    public class CrewMemberListModel
    {
        public int Id { get; set; }
        public string Employee { get; set; }

        public RelationshipType RelationshipType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
