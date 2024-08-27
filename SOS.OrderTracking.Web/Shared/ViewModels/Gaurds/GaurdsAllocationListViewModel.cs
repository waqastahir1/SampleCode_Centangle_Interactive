namespace SOS.OrderTracking.Web.Shared.ViewModels.Gaurds
{

    public class GaurdsAllocationListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string ContactNo { get; set; }

        public int ParentId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? ToPartyId { get; set; }
        public string RegionName { get; set; }
        public string SubRegionName { get; set; }
        public string StationName { get; set; }
        public int? RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }
        public int? ActiveGaurdsCount { get; set; }
        public int? TotalGaurdsRequired { get; set; }
    }

}
