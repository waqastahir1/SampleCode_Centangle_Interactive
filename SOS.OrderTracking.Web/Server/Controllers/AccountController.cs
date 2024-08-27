using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared.ViewModels.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using SOS.OrderTracking.Web.Server.Services;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AccountController> logger;
        private readonly IWebHostEnvironment env;
        private readonly SmtpEmailManager emailManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger, IWebHostEnvironment env, SmtpEmailManager emailManager)
        {
            this.userManager = userManager;
            this.logger = logger;
            this.env = env;
            this.emailManager = emailManager;
        }
     
        [HttpGet]
        [AllowAnonymous]
        public IActionResult QueueWebNotification(int id)
        {
            NotificationAgent.WebPushNotificationsQueue.Add(id);
            return Ok();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email confirmation link");
                return View();
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email confirmation link");
                return View();
            }
             
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join("<br/>", result.Errors.Select(x => x.Description)));
                return View();
            }
            return RedirectToAction(nameof(ChangePassword), new { t = 1, userId = userId, Token = token }); 
        }

        [HttpGet]
        public IActionResult ChangePassword(int t, string userId, string token)
        {
            ViewBag.Message = t == 1 ? "Your email is succesfully confirmed, please create password" : "Please provide new password";
            return View(new UserPasswordViewModel() { UserId = userId, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UserPasswordViewModel viewModel)
        {
            IdentityResult result = null;
            if (viewModel.UserId == null)
            {
                ModelState.AddModelError(string.Empty, "User is not found, this link is tempered");
                return View(viewModel);
            }

            if (string.IsNullOrWhiteSpace(viewModel.Password))
            {
                ModelState.AddModelError(string.Empty, "Password cannot be empty");
                return View(viewModel);
            }


            if (viewModel.Password != viewModel.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(viewModel.ConfirmPassword), "Password and Confirm Password must match");
                return View(viewModel);
            }

            var user = await userManager.FindByIdAsync(viewModel.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User is not valid, this link is tempered");
                return View(viewModel);
            }

            if (user.PasswordHash == null)
            {
                result = await userManager.AddPasswordAsync(user, viewModel.Password);
            }
            else
            {
                if (viewModel.Token == null)
                {
                    ModelState.AddModelError(string.Empty, "Token is not found, this link is tempered");
                    return View(viewModel);
                }
                result = await userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join("<br/>", result.Errors.Select(x => x.Description)));
                return View(viewModel);
            }

            user.ExpireDate = Shared.MyDateTime.Today.AddDays(user.PasswordExpiryInDays);
            result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join("<br/>", result.Errors.Select(x => x.Description)));
                return View(viewModel);
            }
            ViewBag.Message = "Your password is updated successfully";
            return View(new UserPasswordViewModel());
        }

        [HttpGet]
        public IActionResult UserInfo()
        {
            UserInfoViewModel viewModel = new UserInfoViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendEmailForForgetPassword(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest("Please Provide User Name");

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return BadRequest("User is not found");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var changePasswordLink = Url.Action(nameof(ChangePassword), "Account", new { userId = user.Id, token = token }, Request.Scheme);
            logger.LogInformation($"change Password Link : {changePasswordLink}");
            try
            {
                var emailBody = $"This is automated email for password recovery, <a href='{changePasswordLink}'>Click here</a> to create your new password.";
                await emailManager.SendEmail(user.Email, emailBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
            return Ok("Email is sent Please check your inbox");
        }
    }
}
