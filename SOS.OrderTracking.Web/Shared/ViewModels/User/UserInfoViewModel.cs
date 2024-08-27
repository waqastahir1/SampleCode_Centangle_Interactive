using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.User
{
    public class UserInfoViewModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
