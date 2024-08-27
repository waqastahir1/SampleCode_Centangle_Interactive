using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Shared.ViewModels.User;
using System.Security.Cryptography;
using System.Text;

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
        public IActionResult RecoverPassword(string userId, string token = null)
        {
            UserPasswordViewModel viewModel = new UserPasswordViewModel();
            viewModel.UserId = userId;
            viewModel.Token = token;
            return View(viewModel);
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

        public async Task<IActionResult> RecoverPassword(UserPasswordViewModel viewModel)
        {
            try
            {

                IdentityResult result = null;
                if (viewModel.UserId == null)
                    throw new Exception("User not found");

                if (viewModel.Password != viewModel.ConfirmPassword)
                    throw new Exception("Password and Confirm Password must match");

                var user = await userManager.FindByIdAsync(viewModel.UserId);
                if (user == null)
                    throw new Exception("User is not valid");

                var md5Password = MD5Hash(viewModel.Password);

                List<string> passwords = new List<string>();
                if (!string.IsNullOrEmpty(user.PasswordHistory))
                {

                    passwords = JsonConvert.DeserializeObject<List<string>>(user.PasswordHistory);
                    if (passwords.Any(x => x == md5Password))
                    {
                        throw new Exception("You cannot use one of last 5 passwords");
                    }
                }

                passwords.Insert(0, md5Password);
                user.PasswordHistory = JsonConvert.SerializeObject(passwords.Take(5));
                result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    throw new Exception(string.Join("<br/>", result.Errors.Select(x => x.Description)));

                if (user.PasswordHash != null)
                {
                    if (viewModel.Token == null)
                        throw new Exception("Token not found");
                    result = await userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);
                }
                else
                    result = await userManager.AddPasswordAsync(user, viewModel.Password);

                if (!result.Succeeded)
                    throw new Exception(string.Join("<br/>", result.Errors.Select(x => x.Description)));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
                return View(viewModel);
            }

            return Redirect("~/identity/account/signin?returnurl=/");
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }


        [HttpGet]
        public IActionResult ForgotPassword()
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
            var changePasswordLink = Url.Action("RecoverPassword", "Account", new { userId = user.Id, token = token }, Request.Scheme);
            logger.LogInformation($"change Password Link : {changePasswordLink}");
            try
            {
                var emailBody = $"This is automated email for password recovery, <a href='{changePasswordLink}'>Click here</a> to create your new password.";
                await emailManager.SendEmail(user.Email, emailBody, "Password Recovery for SOS Digital CIT Portal");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
            return Ok($"Email sent to {user.Email} Please check your inbox");
        }


        [HttpGet]
        public IActionResult MakeReadonly()
        {
            DatabaseActionController.ChangeReadOnly();
            return Ok();
        }
    }
}
