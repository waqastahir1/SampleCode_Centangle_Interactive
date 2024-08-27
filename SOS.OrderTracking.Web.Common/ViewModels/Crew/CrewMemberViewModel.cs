using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class CrewMemberViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Select Employee")]
        public string Employee { get; set; }

        [Required(ErrorMessage = "Please Select Designation.")]
        public RelationshipType RelationshipType { get; set; }

        [Required(ErrorMessage = "Please Enter Start Date.")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
