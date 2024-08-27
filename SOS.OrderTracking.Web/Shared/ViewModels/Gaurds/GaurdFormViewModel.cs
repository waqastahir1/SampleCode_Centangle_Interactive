using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Gaurds
{
    public class GaurdFormViewModel
    {
        [Required(ErrorMessage = "Please select an employee")]
        public int? Id { get; set; }
        public int RelationshipId { get; set; }
        public string Name { get; set; }
        public int BranchId { get; set; }
        [Required(ErrorMessage = "Please select start date")]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
