using SOS.OrderTracking.Web.Shared.Enums;
using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder
{


    public class VaultNowViewModel
    {
        public int ConsignmentId { get; set; }

        public int DeliveryId { get; set; }

        public int VaultId { get; set; }

        public OrganizationType VaultType { get; set; }

        public IEnumerable<VaultFormListItem> Crews { get; set; }
    }

    public class VaultFormListItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string RegionName { get; set; }

        public string SubRegionName { get; set; }

        public string StationName { get; set; }

    }
}
