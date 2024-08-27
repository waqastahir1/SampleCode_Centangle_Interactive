using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class ChangePasswordViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool IsEnabled { get; set; }

        public int PartyId { get; set; }
        public string RoleId { get; set; }

        public static implicit operator ChangePasswordViewModel(InternalUsersViewModel v)
        {
            throw new NotImplementedException();
        }
    }
}
