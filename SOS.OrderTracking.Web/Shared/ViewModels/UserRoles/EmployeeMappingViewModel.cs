using SOS.OrderTracking.Web.Shared.Enums;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class EmployeeMappingViewModel
    {
        public int? Id { get; set; }
        public int? PartyId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public int? StationId { get; set; }
        public int? SubRegionId { get; set; }
        public int? RegionId { get; set; }
        public string ShortName { get; set; }
        public string FormalName { get; set; }
        public PartyType partyType { get; set; }
    }
}
