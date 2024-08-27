using Microsoft.AspNetCore.Identity;
using SOS.OrderTracking.Web.Common.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class UserRolesViewModel
    {

        readonly UserManager<ApplicationUser> userManager;

        //[Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }

        //[Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        //  [Range(1, int.MaxValue, ErrorMessage = "Please select PartyId")]
        public int PartyId { get; set; }
        public string RoleId { get; set; }
        public List<Party> RolesList { get; set; }
        public MultiSelect2Model multiRole { get; set; }
        // public string[] RolesList { get; set; }
        public bool isAdmin { get; set; }

        public bool isWebUser { get; set; }

        public bool isAppUser { get; set; }

        public IList<string> Roles { get; set; }


        public async Task GetRoleAsync()
        {
            var user = await userManager.FindByNameAsync(UserName);
            var userRoles = await userManager.GetRolesAsync(user);
            List<string> roles = new List<string>();
            foreach (string role in userRoles)
            {
                roles.Add(role);
            }

        }
    }
}
