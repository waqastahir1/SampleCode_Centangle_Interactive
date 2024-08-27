using System.Collections.Generic;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class InternalUsersListModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }

        public IEnumerable<int> OrganizationIds { get; set; }
        public string OrganizationName { get; set; }
        public string Name { get; set; }
        public int PartyId { get; set; }
        public string ShortName { get; set; }
        public int? StationId { get; set; }

        public string Station { get; set; }
    }
}
