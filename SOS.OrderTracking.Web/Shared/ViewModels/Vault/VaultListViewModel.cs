using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.Vault
{
    public class VaultListViewModel
    {
        public int Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }



        public int? RegionId { get; set; }

        public int? SubRegionId { get; set; }

        public int? StationId { get; set; }

        public string RegionName { get; set; }

        public string SubRegionName { get; set; }

        public string StationName { get; set; }

        public string Vehicle { get; set; }

        /// <summary>
        /// No of currently Active members in crew
        /// </summary>
        public int ActiveMembersCount { get; set; }
        public int PresentMembersCount { get; set; }

        public bool HasAccount { get; set; }

        public RelationshipInfo RelationshipInfo { get; set; }

    }

    public class RelationshipInfo
    {
        public DateTime StartDate
        {
            get; set;
        }

        public DateTime? ThruDate
        {
            get; set;
        }

        public bool IsActive { get; set; }
    }
}
