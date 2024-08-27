using SOS.OrderTracking.Web.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultMembersViewModel
    {
        public int Id { get; set; }

        public int VaultId { get; set; }

        [Required(ErrorMessage = "Please Select Employee")]
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Please Select Designation.")]
        public RoleType RelationshipType { get; set; }

        [Required(ErrorMessage = "Please Enter Start Date.")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
