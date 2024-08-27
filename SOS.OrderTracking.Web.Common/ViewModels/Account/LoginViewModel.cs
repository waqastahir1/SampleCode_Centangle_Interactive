using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Common.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
