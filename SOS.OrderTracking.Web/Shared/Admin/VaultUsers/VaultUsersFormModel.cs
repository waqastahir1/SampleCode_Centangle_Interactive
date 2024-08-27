using System;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Admin.VaultUsers
{
    public class VaultUsersFormModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [DataType (DataType.EmailAddress, ErrorMessage ="Provide valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public bool IsEnabled { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select an option")]
        public int PartyId { get; set; }
    }
}
