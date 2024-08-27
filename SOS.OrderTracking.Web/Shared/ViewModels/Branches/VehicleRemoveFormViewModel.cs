using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Branches
{
    public class VehicleRemoveFormViewModel
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public int PartyId { get; set; }

        public DateTime AllocatedFrom { get; set; }

        public DateTime? AllocatedThru { get; set; }

        public string AllocatedBy { get; set; }

        public DateTime AllocatedAt { get; set; }
        public string Vehicle { get; set; }
    }
}
