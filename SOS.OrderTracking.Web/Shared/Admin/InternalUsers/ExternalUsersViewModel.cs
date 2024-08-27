using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.UserRoles
{
    public class ExternalUsersViewModel
    {
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }
        public bool IsEnabled { get; set; }

        // [Range(1, int.MaxValue, ErrorMessage = "Please select PartyId")]
        public int PartyId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string PartyName { get; set; }
    }

}
