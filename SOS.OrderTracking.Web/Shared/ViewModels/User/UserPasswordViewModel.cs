using System.ComponentModel.DataAnnotations;

namespace SOS.OrderTracking.Web.Shared.ViewModels.User
{
    public class UserPasswordViewModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "Minimum 8 characters, at least one uppercase and lowecase letter, one number and one special character")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}
