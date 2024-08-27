using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Data;
using System.Xaml.Permissions;
using SOS.OrderTracking.Web.Shared;

namespace SOS.OrderTracking.Web.Server.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserCacheService userCacheService;
        private readonly AppDbContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
            ILogger<LoginModel> logger,
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

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            //var banks = context.Parties.Where(x => x.SubregionId == 930979 && x.Orgnization.OrganizationType.HasFlag(Shared.Enums.OrganizationType.CustomerBranch)).ToList();
            //foreach (var bank in banks)
            //{
            //    try
            //    {
            //        var n1 = $"{bank.ParentCode.ToLower()}.{bank.ShortName.Split('-').Last()}-01";
            //        if (!context.Users.Any(x => x.UserName == n1))
            //        {

            //            var u1 = new ApplicationUser()
            //            {
            //                UserName = n1,
            //                EmailConfirmed = true,
            //                PhoneNumberConfirmed = true,
            //                PartyId = bank.Id,
            //                CreatedAt = DateTime.Now,
            //                Email = "digitalsupport@sospakistan.net"

            //            };
            //            var r = await _userManager.CreateAsync(u1, "Abc@1234");
            //            if (!r.Succeeded)
            //                throw new Exception($"Errror generating {u1.UserName}, {string.Join(',', r.Errors.Select(x => x.Code + " " + x.Description))}");

            //            r = await _userManager.AddToRoleAsync(u1, "BankBranch");
            //            if (!r.Succeeded)
            //                throw new Exception($"Errror generating {u1.UserName}, {string.Join(',', r.Errors.Select(x => x.Code + " " + x.Description))}");
            //            Console.WriteLine($"Generated {u1.UserName}");
            //        }

            //        var n2 = $"{bank.ParentCode.ToLower()}.{bank.ShortName.Split('-').Last()}-02";
            //        if (!context.Users.Any(x => x.UserName == n2))
            //        {
            //            var u2 = new ApplicationUser()
            //            {
            //                UserName = n2,
            //                EmailConfirmed = true,
            //                PhoneNumberConfirmed = true,
            //                PartyId = bank.Id,
            //                CreatedAt = DateTime.Now,
            //                Email = "digitalsupport@sospakistan.net"
            //            };
            //            var r = await _userManager.CreateAsync(u2, "Abc@1234");
            //            if (!r.Succeeded)
            //                throw new Exception($"Errror generating {u2.UserName}, {string.Join(',', r.Errors.Select(x => x.Code + " " + x.Description))}");

            //            r = await _userManager.AddToRoleAsync(u2, "BankBranchManager");
            //            if (!r.Succeeded)
            //                throw new Exception($"Errror generating {u2.UserName}, {string.Join(',', r.Errors.Select(x => x.Code + " " + x.Description))}");
            //            Console.WriteLine($"Generated {u2.UserName}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Error -> " + ex.Message);
            //    }

            //}


            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //await _signInManager.SignInAsync(await _userManager.FindByNameAsync(Input.Email), false);
                //return LocalRedirect(returnUrl);

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true


#if DEBUG
                var user1 = await _userManager.FindByNameAsync(Input.Email.ToLower());
                await _signInManager.SignInAsync(user1, true);
                await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow);
                return LocalRedirect(returnUrl);
#endif

                var userTemp = await _userManager.FindByNameAsync(Input.Email.ToLower());
                if(userTemp == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username/password");
                    return Page();
                }


                if (userTemp.PasswordExpiryInDays > 0 && userTemp.ExpireDate < MyDateTime.Today)
                {
                    ModelState.AddModelError(string.Empty, "Your Password has expored as per password policy, please you forgot password option to reset your password");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email.ToLower(), Input.Password, Input.RememberMe, lockoutOnFailure: true);
                 
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    await userCacheService.SetSessionTime(Input.Email, DateTime.UtcNow);
                    return LocalRedirect(returnUrl);
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
    }
}
