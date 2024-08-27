using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using SOS.OrderTracking.Web.Shared.ViewModels.Users;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class InternalUsersController : ControllerBase, IInternalUserService
    {
        protected readonly AppDbContext context;
        private readonly ILogger<InternalUsersController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmployeeService employeeService;
        private readonly PartiesCacheService partiesCache;

        public InternalUsersController(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<InternalUsersController> logger, EmployeeService employeeService, PartiesCacheService partiesCache)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
            this.employeeService = employeeService;
            this.partiesCache = partiesCache;
        }
        [HttpGet]
        public async Task<IndexViewModel<InternalUsersListModel>> GetPageAsync([FromQuery] UserAdditionalValueViewModel vm)
        {
            var query = (from u in context.Users
                         from p in context.Parties.Include(x => x.People).Where(x => x.Id == u.PartyId).DefaultIfEmpty()
                         from s in context.People.Where(x => x.Id == p.Id).DefaultIfEmpty()
                         from r in context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                         orderby u.UserName
                         select new
                         {
                             u.Id,
                             u.UserName,
                             u.Email,
                             u.PartyId,
                             r.RoleId,
                             s.NationalId,
                             p.StationId,
                             p.SubregionId,
                             p.RegionId,
                             p.ShortName,
                             p.FormalName,
                             p.PartyType,
                             p.People.DesignationDesc,
                             p.Orgnization.OrganizationType,
                             OrgIds = context.PartyRelationships.Where(x=>x.FromPartyId == p.Id && x.IsActive)
                             .Select(x=>x.ToPartyId).ToList()
                         });
            query = query.Where(x => x.PartyType == PartyType.Person || x.OrganizationType == OrganizationType.Crew);
          
            if (!string.IsNullOrEmpty(vm.SearchKey))
            {
                query = query.Where(x => x.FormalName.ToLower().Contains(vm.SearchKey.ToLower())
                || x.UserName.ToLower().Contains(vm.SearchKey.ToLower())
                || x.ShortName.ToLower().Contains(vm.SearchKey.ToLower())
                || x.NationalId.ToLower().Contains(vm.SearchKey.ToLower()));
            }
            else
            {
                if (vm.RegionId.GetValueOrDefault() > 0)
                {
                    query = query.Where(x => x.RegionId == vm.RegionId);// || !x.RegionId.HasValue);
                }
                if (vm.SubRegionId.GetValueOrDefault() > 0)
                {
                    query = query.Where(x => x.SubregionId == vm.SubRegionId);// || !x.SubregionId.HasValue);
                }
                if (vm.StationId.GetValueOrDefault() > 0)
                {
                    query = query.Where(x => x.StationId == vm.StationId);// || !x.StationId.HasValue);
                }

                if (!string.IsNullOrEmpty(vm.RoleTypeId))
                {
                    query = query.Where(x => x.RoleId == vm.RoleTypeId);
                }
            }

            var totalRows = query.Count();

            var items = await query
                .Select(x => new InternalUsersListModel()
                {
                    Id = x.Id,
                    PartyId = x.PartyId,
                    Name = x.FormalName + "( " + x.DesignationDesc + ")",
                    ShortName = x.ShortName,
                    UserName = x.UserName,
                    Email = x.Email,
                    RoleName = context.Roles.FirstOrDefault(y => y.Id == x.RoleId).Name,
                    OrganizationIds = x.OrgIds,
                    StationId = x.StationId
                })
                .Skip((vm.CurrentIndex -1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            //getting relationships of users
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var user in items)
            {
                var relation = await employeeService.RelationshipDetail(user.PartyId, MyDateTime.Now);

                if (relation == null)
                {
                    relation = new RelationshipDetailViewModel();
                }
                if (!dict.ContainsKey(user.Id))
                {
                    dict.Add(user.Id, relation.OrganizationName);
                }
            }

            foreach (var item in items)
            {
                item.OrganizationName = string.Join(',', item.OrganizationIds.Select(p => partiesCache.GetName(p).Result));
                item.Station = partiesCache.GetName(item.StationId).Result;
            }

            return new IndexViewModel<InternalUsersListModel>(items, totalRows);
        }

        [HttpGet]
        public async Task<List<InternalUsersViewModel>> GetRolesAsync()
        {
            var roles = new List<InternalUsersViewModel>()
            {
                new InternalUsersViewModel() {  RoleId = "CIT", RoleName = "CIT" },
                new InternalUsersViewModel() {  RoleId = "CPC", RoleName = "CPC" },
                new InternalUsersViewModel() {  RoleId = "SOS-Admin", RoleName = "SOS-Admin" },
                new InternalUsersViewModel() {  RoleId = "SOS-Regional-Admin", RoleName = "SOS-Regional-Admin" },
                new InternalUsersViewModel() {  RoleId = "SOS-SubRegional-Admin", RoleName = "SOS-SubRegional-Admin" },
                new InternalUsersViewModel() {  RoleId = "VAULT", RoleName = "VAULT" },
                new InternalUsersViewModel() {  RoleId = "SOS-Headoffice-Billing", RoleName = "SOS-Headoffice-Billing" },
                new InternalUsersViewModel() {  RoleId = "Edit-Distance-Draft", RoleName = "Edit-Distance-Draft" },
                new InternalUsersViewModel() {  RoleId = "Edit-Distance-Approve", RoleName = "Edit-Distance-Approve" },
                new InternalUsersViewModel() {  RoleId = "Super-Admin", RoleName = "Super-Admin" }
                };

            return roles;
        }

        [HttpGet]
        public async Task<InternalUsersViewModel> GetAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var users = await (from u in context.Users
                               where u.Id == id
                               select new InternalUsersViewModel()
                               {
                                   Id = u.Id,
                                   UserName = u.UserName,
                                   RoleName = string.Join(",",userManager.GetRolesAsync(user).Result),
                                   //RoleId = context.UserRoles.FirstOrDefault(x => x.UserId == u.Id).RoleId,
                                   PartyId = u.PartyId,
                                   IsEnabled = u.EmailConfirmed
                               }).FirstAsync();

            return users;
        }

        [HttpPost]
        public async Task<string> PostAsync(InternalUsersViewModel SelectedItem)
        {
            try
            {
                IdentityResult result = null;
                var applicationUser = string.IsNullOrEmpty(SelectedItem.Id) ? new ApplicationUser
                {
                    UserName = SelectedItem.UserName,
                    Email = SelectedItem.UserName,
                    CreatedAt = DateTime.Now
                } : await userManager.FindByIdAsync(SelectedItem.Id);


                if (applicationUser == null)
                    NotFound("User not found");

                applicationUser.UserName = SelectedItem.UserName;
                applicationUser.Email = SelectedItem.UserName;
                applicationUser.PartyId = SelectedItem.PartyId;
                applicationUser.EmailConfirmed = SelectedItem.IsEnabled;
                if (string.IsNullOrEmpty(SelectedItem.Id))
                {
                    result = await userManager.CreateAsync(applicationUser, SelectedItem.Password);
                }
                else
                {
                    result = await userManager.UpdateAsync(applicationUser);
                }
                if (result.Succeeded)
                {
                        string[] roles = SelectedItem.RoleId.Split(',');
                        foreach (var role in roles)
                        {
                            await userManager.AddToRoleAsync(applicationUser, role);
                        }
                }
                else
                {
                    throw new BadRequestException(string.Join("<br/>", result.Errors.Select(x => x.Description)));
                }
                return "{}"; //applicationUser.Id;
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
                throw new BadRequestException(result.Errors.Select(x => x.Description).FirstOrDefault());

            return result.Succeeded;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetEmployees(string userId)
        {
            var users = await (from p in context.Parties
                               join r in context.People on p.Id equals r.Id
                               select new SelectListItem()
                               {
                                   IntValue = p.Id,
                                   Text = p.ShortName + " " + p.FormalName + " " + r.NationalId + " " + r.DesignationDesc
                               }).ToListAsync();
            var loginUsers = await (from p in context.Parties
                                    join r in context.People on p.Id equals r.Id
                                    join u in context.Users on p.Id equals u.PartyId
                                    select new SelectListItem()
                                    {
                                        IntValue = p.Id,
                                        Text = p.ShortName
                                    }).ToListAsync();
            users.RemoveAll(c => loginUsers.ToList().Exists(n => n.IntValue == c.IntValue));
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await (from p in context.Parties
                                  join r in context.People on p.Id equals r.Id
                                  join u in context.Users on p.Id equals u.PartyId
                                  where u.Id == userId
                                  select new SelectListItem()
                                  {
                                      IntValue = p.Id,
                                      Text = p.ShortName + " " + p.FormalName + " " + r.NationalId + " " + r.DesignationDesc
                                  }).FirstOrDefaultAsync();
                users.Add(user);
            }
            return users;
        }
    }
}
