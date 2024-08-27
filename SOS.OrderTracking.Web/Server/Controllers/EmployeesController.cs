using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;

namespace SOS.OrderTracking.Web.Server.Controllers
{

    [Route("v1/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class EmployeesController : ControllerBase , IEmployeesService
    {
        protected readonly AppDbContext context;
        private readonly ILogger<InternalUsersController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmployeeService employeeService;
        public EmployeesController(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<InternalUsersController> logger, EmployeeService employeeService)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
            this.employeeService = employeeService;
        }

        public Task<EmployeesViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IndexViewModel<EmployeesListViewModel>> GetPageAsync([FromQuery] EmployeesAdditionalValueViewModel vm)
        {
            var query = (from p in context.Parties
                         from u in context.Users.Where(x => x.PartyId == p.Id).DefaultIfEmpty()
                         from r in context.UserRoles.Where(x => x.UserId == u.Id).DefaultIfEmpty()
                     
                         select new EmployeeMappingViewModel
                         {
                             Id = p.Id,//p.Id.Equals(null) == true ? 0 : p.Id,
                             PartyId = u.PartyId,//u.PartyId.Equals(null) == true ?  0 : u.PartyId,
                             Username = u.UserName,
                             Email = u.Email,
                             RoleId = r.RoleId,
                             StationId = p.StationId,
                             SubRegionId = p.SubregionId,
                             RegionId = p.RegionId,
                             ShortName = p.ShortName,
                             FormalName = p.FormalName,
                             partyType = p.PartyType
                         });

            query = query.Where(x => x.partyType == PartyType.Person);
            if (vm.RegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.RegionId == vm.RegionId);// || !x.RegionId.HasValue);
            }
            if (vm.SubRegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.SubRegionId == vm.SubRegionId);// || !x.SubregionId.HasValue);
            }
            if (vm.StationId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.StationId == vm.StationId);// || !x.StationId.HasValue);
            }


            if (!string.IsNullOrEmpty(vm.searchKey))
            {
                query = query.Where(x => x.FormalName.ToLower().Contains(vm.searchKey.ToLower())
                || x.ShortName.ToLower().Contains(vm.searchKey.ToLower()));
            }
            await query.ToListAsync();
            Dictionary<int, string> dict = new Dictionary<int, string>();
            RelationshipDetailViewModel relation = null;
            foreach (var user in query)
            {
                relation = await employeeService.RelationshipDetail(user.PartyId.HasValue == true ? user.PartyId.Value : 0, MyDateTime.Now);

                if (relation == null)
                {
                    relation = new RelationshipDetailViewModel();
                }
                if (!dict.ContainsKey(user.Id.HasValue == true ? user.Id.Value : 0))
                {
                    dict.Add(user.Id.HasValue == true ? user.Id.Value : 0, relation.OrganizationName);
                }
            }
            var totalRows = query.Count();

            var items = await query
                .Select(x => new EmployeesListViewModel()
                {
                    UserId = x.Id.HasValue == true ? x.Id.Value : 0,
                    EmployeeName = x.FormalName,
                    EmployeeCode = x.ShortName,
                    UserName = x.Username,
                    Email = x.Email,
                    PartyId = x.PartyId,//x.PartyId.HasValue == true ? x.PartyId.Value : 0,
                    Role = (from r in context.Roles
                           where r.Id == (x.RoleId == null ? string.Empty : x.RoleId)
                           select r.Name).ToList()//context.Roles.Where(y => y.Id == x.RoleId).Select(x => x.Name),
                    // OrganizationName = dict.FirstOrDefault(k => k.Key.Equals(x.Id)).Value
                })
                .Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            foreach (var item in items)
            {
                item.OrganizationName = dict.FirstOrDefault(x => x.Key == item.UserId).Value;
                if (string.IsNullOrEmpty(item.UserName))
                    item.TickColor = "orange";
                else
                    item.TickColor = "green";
            }

            return new IndexViewModel<EmployeesListViewModel>(items, totalRows);
        }

        public Task<int> PostAsync(EmployeesViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
