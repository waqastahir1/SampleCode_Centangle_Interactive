using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Branches
{
    public class VehicleToBranchFormViewModel
    {
        [Required]
        public int PartyId { get; set; }
        [Required(ErrorMessage = "Please Select Vehicle")]
        public int? AssetId { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AllocatedBy { get; set; }
        public DateTime? AllocatedAt { get; set; }
        public int AssetAllocationId { get; set; }
        public string Vehicle { get; set; } //Just for showing property to user only
    }
}
