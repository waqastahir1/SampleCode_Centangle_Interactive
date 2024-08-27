using Microsoft.AspNetCore.Identity;
using SOS.OrderTracking.Web.Common.Data.Models;
using System.Collections.Generic;


namespace SOS.OrderTracking.Web.Common.ViewModels
{
    public class UserRolesListModel
    {

        readonly UserManager<ApplicationUser> userManager;

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public string Password { get; set; }

        public int PartyId { get; set; }
        public string RoleId { get; set; }
        public List<Party> RolesList { get; set; }
        // public string[] RolesList { get; set; }
        public MultiSelect2Model multiRole { get; set; }
        public bool isAdmin { get; set; }

        public bool isWebUser { get; set; }

        public bool isAppUser { get; set; }

        public IList<string> Roles { get; set; }



    }
}
