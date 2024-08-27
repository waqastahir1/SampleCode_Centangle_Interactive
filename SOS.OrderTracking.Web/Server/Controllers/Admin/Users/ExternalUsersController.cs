using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using SOS.OrderTracking.Web.Shared.ViewModels.Users;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ExternalUsersController : ControllerBase ,IExternalUserService
    {
        protected AppDbContext Context { get; set; }
        private readonly ILogger<ExternalUsersController> logger;
        private UserManager<ApplicationUser> userManager { get; set; }
        public ExternalUsersController(AppDbContext appDbContext, UserManager<ApplicationUser> userManager)
        {
            Context = appDbContext;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IndexViewModel<ExternalUsersListViewModel>> GetPageAsync([FromQuery] UserAdditionalValueViewModel vm)
        {
            var query = (from u in Context.Users
                         from r in Context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                         from p in Context.Parties.Where(x => x.Id == u.PartyId)
                         where p.PartyType != PartyType.Person
                         select new
                         {
                             u.Id,
                             u.UserName,
                             u.Email,
                             r.RoleId,
                             p.StationId,
                             p.SubregionId,
                             p.RegionId,
                             p.ShortName,
                             p.FormalName,
                             p.PartyType,
                             u.PartyId
                         });

        
            if (vm.MainCustomerId > 0)
            {
                query = (from p in Context.Parties
                         join r in Context.PartyRelationships on p.Id equals r.FromPartyId
                         join u in Context.Users on p.Id equals u.PartyId
                         from d in Context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                         where (r.ToPartyId == vm.MainCustomerId || u.PartyId == vm.MainCustomerId)
                         && (vm.RoleTypeId == null || vm.RoleTypeId == d.RoleId) 
                         && p.PartyType != PartyType.Person
                         
                         select new
                         {
                             u.Id,
                             u.UserName,
                             u.Email,
                             d.RoleId,
                             p.StationId,
                             p.SubregionId,
                             p.RegionId,
                             p.ShortName,
                             p.FormalName,
                             p.PartyType,
                             u.PartyId
                         });

            }

            if (vm.RegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.RegionId == null || x.RegionId == vm.RegionId);
            }

            if (vm.SubRegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.SubregionId == vm.SubRegionId);
            }
            if (vm.StationId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.StationId == vm.StationId);
            }

            if (!string.IsNullOrEmpty(vm.RoleTypeId))
            {
                query = query.Where(x => x.RoleId == vm.RoleTypeId);
            }
            if (!string.IsNullOrEmpty(vm.SearchKey))
            {
                query = query.Where(x => x.FormalName.ToLower().Contains(vm.SearchKey.ToLower())
                || x.ShortName.ToLower().Contains(vm.SearchKey.ToLower()));
            }

            var totalRows = query.Count();

            var items = await query
                .Select(x => new ExternalUsersListViewModel()
                {
                    Id = x.Id,
                    Name = x.FormalName,
                    shortName = x.ShortName,
                    UserName = x.UserName,
                    Email = x.Email
                })
                .Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<ExternalUsersListViewModel>(items, totalRows);
        }

        [HttpGet]
        public async Task<ExternalUsersViewModel> GetAsync(string id)
        {
            var users = await (from u in Context.Users
                               join p in Context.Parties on u.PartyId equals p.Id
                               where u.Id == id
                               select new ExternalUsersViewModel()
                               {
                                   Id = u.Id,
                                   UserName = u.UserName,
                                   RoleId = Context.UserRoles.FirstOrDefault(x => x.UserId == u.Id).RoleId,
                                   RoleName = Context.Roles.FirstOrDefault(x => x.Id == id).Name,
                                   PartyId = u.PartyId,
                                   PartyName = p.FormalName,
                                   IsEnabled = u.EmailConfirmed
                               }).FirstAsync();

            return users;
        }

        [HttpPost]
        public async Task<string> PostAsync(ExternalUsersViewModel SelectedItem)
        {
            try
            {
                IdentityResult result = null;
                var applicationUser = await userManager.FindByNameAsync(SelectedItem.UserName.ToLower());
                if (applicationUser == null)
                {
                    applicationUser = new ApplicationUser();
                }

                applicationUser.UserName = SelectedItem.UserName;
                applicationUser.Email = SelectedItem.UserName + "@SOS.com";
                applicationUser.PartyId = SelectedItem.PartyId;
                applicationUser.EmailConfirmed = SelectedItem.IsEnabled;
                applicationUser.CreatedAt = DateTime.Now;
                if (SelectedItem.Password != null)
                {
                    applicationUser.PasswordHash = userManager.PasswordHasher.HashPassword(applicationUser, SelectedItem.Password);
                }

                if (string.IsNullOrEmpty(SelectedItem.Id))
                {
                    result = await userManager.CreateAsync(applicationUser, SelectedItem.Password);
                    if (!result.Succeeded)
                        throw new BadRequestException(string.Join("<br/>", result.Errors.Select(x => x.Description)));
                }
                else
                {
                    result = await userManager.UpdateAsync(applicationUser);
                    if (!result.Succeeded)
                        throw new BadRequestException(string.Join("<br/>", result.Errors.Select(x => x.Description)));
                }
                if (result.Succeeded)
                {
                    var RoleName = Context.Roles.FirstOrDefault(x => x.Id == SelectedItem.RoleId).Name;
                    var allAssignedRoles = await userManager.GetRolesAsync(applicationUser);
                    _ = await userManager.RemoveFromRolesAsync(applicationUser, allAssignedRoles.ToArray());
                    result = await userManager.AddToRoleAsync(applicationUser, RoleName);
                    if (result.Succeeded)
                    {
                        return "a";
                    }

                }

                if (result.Succeeded)
                {
                    return "a";
                }
                else
                {
                    throw new BadRequestException(result.Errors.ToString());
                }

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
                         where o.OrganizationType == OrganizationType.MainCustomer  || o.OrganizationType == OrganizationType.Customer
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
                             where o.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
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

        Task<bool> IExternalUserService.ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            throw new NotImplementedException();
        }
         
    }
}
