using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Admin.BankUsersDetail;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using SOS.OrderTracking.Web.Shared.ViewModels.Users;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class BankUsersDetailController : ControllerBase , IBankUsersDetailService
    {
        protected AppDbContext Context { get; set; }
        private readonly ILogger<ExternalUsersController> logger;
        private readonly SmtpEmailManager emailManager;

        private UserManager<ApplicationUser> userManager { get; set; }
        private readonly IWebHostEnvironment env;

        public BankUsersDetailController(AppDbContext appDbContext, UserManager<ApplicationUser> userManager, SmtpEmailManager emailManager,
            IWebHostEnvironment env, ILogger<ExternalUsersController> logger)
        {
            Context = appDbContext;
            this.userManager = userManager;
            this.emailManager = emailManager;
            this.env = env;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IndexViewModel<BankUserDetailListViewModel>> GetPageAsync([FromQuery] UserAdditionalValueViewModel vm)
        {
            var query = (from u in Context.Users
                         from d in Context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                         where (u.PartyId == vm.BankBranchId)

                         select new
                         {
                             u.Id,
                             u.UserName,
                             u.Email,
                             d.RoleId,
                             u.PartyId,
                             u.PasswordHash,
                             u.ExpireDate,
                             u.PasswordExpiryInDays,
                             u.Name,
                             u.PhoneNumberConfirmed,
                             u.EmailConfirmed
                         });


            var totalRows = query.Count();

            var items = await query
                .Select(x => new BankUserDetailListViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UserName = x.UserName,
                    Email = x.Email,
                    ExpiryDate = x.ExpireDate,
                    ExpiryPolicy = x.PasswordExpiryInDays,
                    HasPassord = x.PasswordHash != null,
                    EmailConfirmed= x.EmailConfirmed,
                    IsActive = x.PhoneNumberConfirmed,
                    
                    Role = x.RoleId
                })
                .Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<BankUserDetailListViewModel>(items, totalRows);
        }

        [HttpGet]
        public async Task<BankUserDetailFormViewModel> GetAsync(string id)
        {
            var users = await (from u in Context.Users
                               join p in Context.Parties on u.PartyId equals p.Id
                               join r in Context.UserRoles on u.Id equals r.UserId
                               where u.Id == id
                               select new BankUserDetailFormViewModel()
                               {
                                   Id = u.Id,
                                   UserName = u.UserName,
                                   EmailAddress = u.Email,
                                   PartyId = u.PartyId,
                                   Name = u.Name,
                                   RoleId = r.RoleId,
                                   ExpiryPolicy = u.PasswordExpiryInDays,
                                   IsActive = u.PhoneNumberConfirmed
                               }).FirstAsync();

            return users;
        }

        private bool isEmailValid(string pEmail)
        {
            return Regex.IsMatch(pEmail,
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        [HttpPost]
        public async Task<string> PostAsync(BankUserDetailFormViewModel selectedItem)
        {
            try
            {
                IdentityResult result = null; 
                    if (string.IsNullOrEmpty(selectedItem.EmailAddress) || !isEmailValid(selectedItem.EmailAddress))
                        throw new BadRequestException("Provided Email is not Valid");

                var applicationUser = string.IsNullOrEmpty(selectedItem.Id) ? new ApplicationUser
                {
                    CreatedAt = DateTime.Now
                } : await userManager.FindByIdAsync(selectedItem.Id);



                var previousEmail = applicationUser.Email;
                var previousUserName = applicationUser.UserName;

                applicationUser.UserName = selectedItem.UserName;
                applicationUser.Email = selectedItem.EmailAddress;
                applicationUser.Name = selectedItem.Name;
                applicationUser.PartyId = selectedItem.PartyId;
                applicationUser.PasswordExpiryInDays = selectedItem.ExpiryPolicy;
                applicationUser.ExpireDate = MyDateTime.Today.AddDays(selectedItem.ExpiryPolicy);
                applicationUser.PhoneNumberConfirmed = selectedItem.IsActive;
                
                if (string.IsNullOrEmpty(previousUserName))
                    result = await userManager.CreateAsync(applicationUser);
                else
                {
                    if (previousEmail != applicationUser.Email)
                        applicationUser.EmailConfirmed = false;

                    result = await userManager.UpdateAsync(applicationUser);

                    var roles = Context.UserRoles.Where(x => x.UserId == applicationUser.Id)
                        .Select(x => x.RoleId)
                        .ToArray();
                    await userManager.RemoveFromRolesAsync(applicationUser, roles);
                }

                if (!result.Succeeded)
                    throw new BadRequestException(string.Join("<br/>", result.Errors.Select(x => x.Description)));

               
                result = await userManager.AddToRoleAsync(applicationUser, selectedItem.RoleId);

                if (!result.Succeeded)
                    throw new BadRequestException(string.Join("<br/>", result.Errors.Select(x => x.Description)));

                if (!applicationUser.EmailConfirmed)
                {
                    await userManager.RemovePasswordAsync(applicationUser);
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    string emailConfirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = applicationUser.Id, token = token }, Request.Scheme);
                    logger.LogInformation($"Email Confirmation Link with getting password : {emailConfirmationLink}");

                    await emailManager.SendFormattedEmail(applicationUser.Email, env, selectedItem.RoleId, emailConfirmationLink, applicationUser.UserName);
                }


                return applicationUser.PartyId.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw new BadRequestException(ex.Message);
            }

        }
        [HttpPost]
        public async Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            var user = await userManager.FindByIdAsync(changePasswordViewModel.Id);
            await userManager.RemovePasswordAsync(user);
            var result = await userManager.AddPasswordAsync(user, changePasswordViewModel.Password);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join("<br>", result.Errors.Select(x => x.Description)));

            return result.Succeeded;
        }
        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetMainCustomers()
        {
            int? parentId = null;
            if (User.IsInRole("BANK"))
            {
                parentId = (await Context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name)).PartyId;
            }
            var query = (from o in Context.Orgnizations
                         join p in Context.Parties on o.Id equals p.Id
                         where o.OrganizationType == OrganizationType.MainCustomer
                         && (parentId == null || o.Id == parentId)
                         select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName));
            return await query.ToArrayAsync();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<SelectListItem>> SearchCustomers(string search)
        {
            try
            {
                if (search == null)
                    return Array.Empty<SelectListItem>();

                search = search.ToLower();
                var query = (from o in Context.Orgnizations
                             join p in Context.Parties on o.Id equals p.Id
                             where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                             && (p.ShortName.ToLower().Contains(search) || p.FormalName.ToLower().Contains(search))
                             select new SelectListItem(o.Id, p.ShortName + "-" + p.FormalName, p.Abbrevation));

                var banks = await query.Take(20).ToArrayAsync();
                return banks;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
