using System;

namespace SOS.OrderTracking.Web.Shared.Admin.BankUsersDetail
{
    public class BankUserDetailListViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public int ExpiryPolicy { get; set; }

        public bool HasPassord { get; set; }

        public bool IsActive { get; set; }

        public bool EmailConfirmed { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
