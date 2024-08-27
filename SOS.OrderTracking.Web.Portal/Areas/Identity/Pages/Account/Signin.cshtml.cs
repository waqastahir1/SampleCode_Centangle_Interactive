using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared;
using System.ComponentModel.DataAnnotations;
using static SOS.OrderTracking.Web.Shared.Constants;

namespace SOS.OrderTracking.Web.Portal.Areas.Identity.Pages
{
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public class SigninModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserCacheService userCacheService;
        private readonly AppDbContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<SigninModel> _logger;

        public SigninModel(SignInManager<ApplicationUser> signInManager,
            ILogger<SigninModel> logger,
            UserManager<ApplicationUser> userManager,
            UserCacheService userCacheService,
            AppDbContext context)
        {
            _userManager = userManager;
            this.userCacheService = userCacheService;
            this.context = context;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl = returnUrl == null || returnUrl == "/" ? Url.Content("~/shipments/live") : returnUrl;

            if (ModelState.IsValid)
            {

                var userTemp = await _userManager.FindByNameAsync(Input.Email.ToLower());
                if (userTemp == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/password");
                    return Page();
                }
                var emailConf = await _userManager.IsEmailConfirmedAsync(userTemp);
                if (!emailConf)
                {
                    ModelState.AddModelError(string.Empty, "User is disabled");
                    return Page();
                }
#if DEBUG
                await _signInManager.SignInAsync(userTemp, true);
                var roles = await _userManager.GetRolesAsync(userTemp);
                if (roles.Any(x => x == "CIT")) return Redirect("~/Crew/Information");
                return Redirect(returnUrl);
#endif

                if (userTemp.PasswordExpiryInDays > 0 && userTemp.ExpireDate < MyDateTime.Today)
                {
                    ModelState.AddModelError(string.Empty, "Your Password has expired as per password policy, please you forgot password option to reset your password");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email.ToLower(), Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await ForceLogoutUser(Input.Email);
                    _logger.LogInformation("User logged in.");
                    var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

                    await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow.AddHours(5));
                    await userCacheService.SetUserIp(Input.Email, remoteIpAddress);
                    var _roles = await _userManager.GetRolesAsync(userTemp);
                    if (_roles.Any(x => x == "CIT")) return Redirect("~/Crew/Information");
                    return Redirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public async Task<bool> ForceLogoutUser(string UserName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(UserName);
                await PubSub.Hub.Default.PublishAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
