using SOS.OrderTracking.Web.Shared.Enums;
using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class CrewMemberListModel
    {
        public int Id { get; set; }
        public int CrewId { get; set; }
        public string CrewName { get; set; }
        public string Station { get; set; }

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public RoleType RelationshipType { get; set; }
        public string Designation { get { return RelationshipType.ToString(); } }

        public string NationalId { get; set; }

        public DateTime StartDate { get; set; }
        public string StartDateStr { get { return StartDate.ToString("dd-MM-yy hh:mm tt"); } }

        public DateTime? EndDate { get; set; }
        public string EndDateStr { get { return EndDate?.ToString("dd-MM-yy hh:mm tt"); } }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }

        public string ImageLink { get; set; }

        public bool IsActive { get; set; }
    }
}
