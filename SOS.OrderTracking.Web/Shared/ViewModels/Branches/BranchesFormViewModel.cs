using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Branches
{
    public class BranchesFormViewModel
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        [Range(1, 9, ErrorMessage = "Dedicated Vehicle Capacity should be between 1 to 9")]
        public int? DedicatedVehicleCapacity { get; set; }
        [Required(ErrorMessage = "Please select start date")]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
