using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Index.HPRtree;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.BankUser;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Admin
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class BankUsersController : ControllerBase, IBankUserService
    {
        private readonly AppDbContext context;
        private UserManager<ApplicationUser> userManager;
        private readonly SmtpEmailManager emailManager;
        private ILogger<BankUsersController> logger;
        private readonly IWebHostEnvironment env;

        public BankUsersController(ILogger<BankUsersController> logger, IWebHostEnvironment env,
            AppDbContext context, UserManager<ApplicationUser> userManager, SmtpEmailManager emailManager)
        {
            this.logger = logger;
            this.env = env;
            this.userManager = userManager;
            this.emailManager = emailManager;
            this.context = context;
        }
        [HttpGet]
        public Task<BankUserFormViewModel> GetAsync(string id)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<BankUserFormViewModel> GetUserAsync(string id, int roleType)
        {
            BankUserFormViewModel user = null;
            if (roleType == 1)
            {
                user = await (from u in context.Users
                              from r in context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                              where u.Id == id
                              && r.RoleId == "BankBranchManager"
                              select new BankUserFormViewModel()
                              {
                                  SupervisorUserId = u.Id,
                                  SupervisorUserName = u.UserName,
                                  IsSupervisorEnabled = u.EmailConfirmed,
                                  RoleName = context.Roles.FirstOrDefault(x => x.Id == r.RoleId).Name,
                                  PartyId = u.PartyId,
                                  SupervisorEmail = u.Email
                              }).FirstOrDefaultAsync();
            }
            else
            {
                user = await (from u in context.Users
                              from r in context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                              where u.Id == id
                              && r.RoleId == "BankBranch"
                              select new BankUserFormViewModel()
                              {
                                  InitiatorUserId = u.Id,
                                  InitiatorUserName = u.UserName,
                                  IsInitiatorEnabled = u.EmailConfirmed,
                                  RoleName = context.Roles.FirstOrDefault(x => x.Id == r.RoleId).Name,
                                  PartyId = u.PartyId,
                                  InitiatorEmail = u.Email
                              }).FirstOrDefaultAsync();
            }
            return user;
        }
        [HttpGet]
        public async Task<IndexViewModel<BankUserListViewModel>> GetPageAsync([FromQuery] BankUsersAdditionalValueViewModel vm)
        {

            if (vm.RegionId.GetValueOrDefault() == 0)
                throw new NotFoundException("No Content Found");


            var query = (from p in context.Parties
                         from m in context.PartyRelationships.Where(x => x.FromPartyId == p.Id).DefaultIfEmpty()
                         where p.Orgnization.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
                          && (vm.RegionId == null || vm.RegionId == p.RegionId)
                          && (vm.SubRegionId == null || vm.SubRegionId == p.SubregionId)
                          && (vm.StationId == null || vm.StationId == p.StationId)
                         select new BankUserListViewModel()
                         {
                             PartyId = p.Id,
                             ToPartyId = m.ToPartyId,
                             BranchCode = p.ShortName,
                             BranchName = p.FormalName,
                             RegionId = p.RegionId,
                             SubRegionId = p.SubregionId,
                             StationId = p.StationId, 
                             Users = (from u in context.Users
                                          join ur in context.UserRoles on u.Id equals ur.UserId
                                          where  u.PartyId == p.Id
                                          select ur.RoleId).ToList(),
                         });

            if (!string.IsNullOrEmpty(vm.SortColumn))
            {
                query = query.OrderBy(vm.SortColumn);
            }
            if (vm.MainCustomerId > 0)
            {
                query = query.Where(x => x.ToPartyId == vm.MainCustomerId);
            }
            var totalRows = await query.CountAsync();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<BankUserListViewModel>(items, totalRows);
        }
        [HttpPost]
        public Task<string> PostAsync([FromBody] BankUserFormViewModel selectedItem)
        {
            throw new  NotImplementedException();
        }
        private bool isEmailValid(string pEmail)
        {
            return Regex.IsMatch(pEmail,
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

    }
}
