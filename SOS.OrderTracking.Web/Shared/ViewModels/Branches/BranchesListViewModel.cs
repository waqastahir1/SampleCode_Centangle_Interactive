namespace SOS.OrderTracking.Web.Shared.ViewModels.Branches
{
    public class BranchesListViewModel
    {
        public int Id { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public int? ToPartyId { get; set; }
        public string RegionName { get; set; }
        public string SubRegionName { get; set; }
        public string StationName { get; set; }
        public int? RegionId { get; set; }
        public int? SubRegionId { get; set; }
        public int? StationId { get; set; }
        public byte? DedicatedVehicleCapacity { get; set; }
        public int? ActiveVehiclesCount { get; set; }

        public decimal? Radius { get; set; }
    }
}
