using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.Admin
{
    public class RolesViewModel
    {
        [Required(ErrorMessage = "Please select Role")]
        public IEnumerable<string> RoleIds { get; set; }
        public string UserId { get; set; }
    }
}
