using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Branches
{
    public class BranchVehicleListViewModel
    {
        public int AllocationId { get; set; }
        public string Asset { get; set; } //Vehicle MNS-17-908
        public string FormalName { get; set; } // /KHI/KHI/MNS-17-908
        public DateTime? AllocatedFrom { get; set; }
        public DateTime? AllocatedThru { get; set; }
        public bool IsActive { get; set; }
        public string AllocatedBy { get; set; }

    }
}
