using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SOS.OrderTracking.Web.Common.Data.Models;

namespace SOS.OrderTracking.Web.Server.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public async Task OnGet()
        {
            await signInManager.SignOutAsync();
        }
    }
}
