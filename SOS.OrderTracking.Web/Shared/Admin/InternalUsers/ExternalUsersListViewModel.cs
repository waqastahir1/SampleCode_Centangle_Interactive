using System;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class ExternalUsersListViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int PartyId { get; set; }
        public string shortName { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
