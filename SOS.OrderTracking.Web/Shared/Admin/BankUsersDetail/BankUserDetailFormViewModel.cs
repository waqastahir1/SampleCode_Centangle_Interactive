using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Admin.BankUsersDetail
{
    public class BankUserDetailFormViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Range(minimum: 1, maximum: 360)]
        public int ExpiryPolicy { get; set; }

        public int PartyId { get; set; }

        public string RoleId { get; set; }

        public bool AutoName { get; set; }
    }
}
