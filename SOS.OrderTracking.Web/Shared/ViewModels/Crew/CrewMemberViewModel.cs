using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels
{
    public class CrewMemberViewModel
    {
        public int Id { get; set; }

        public int CrewId { get; set; }

        public bool ChangeRelationship { get; set; }
        public string EmployeeName { get; set; }
        [Required(ErrorMessage = "Please Select Employee")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select Employee")]
        public int EmployeeId { get; set; }
        [Required(ErrorMessage = "Please Select Designation.")]
        public RoleType RelationshipType { get; set; }

        [Required(ErrorMessage = "Please Enter Start Date.")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
