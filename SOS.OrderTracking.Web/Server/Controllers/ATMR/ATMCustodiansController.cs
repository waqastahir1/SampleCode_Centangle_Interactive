using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ATMCustodiansController : ControllerBase ,IATMCustodiansService
    {

        private readonly AppDbContext context;

        private readonly SequenceService sequenceService;
        private readonly EmployeeService peopleService;
        private UserManager<ApplicationUser> userManager { get; set; }
        public ATMCustodiansController(AppDbContext appDbContext,
            SequenceService sequenceService, EmployeeService peopleService, UserManager<ApplicationUser> userManager)
        {
            context = appDbContext;
            this.sequenceService = sequenceService;
            this.peopleService = peopleService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IndexViewModel<ATMCustodiansListViewModel>> GetPageAsync([FromQuery]BaseIndexModel vm)
        {
            var query = (from p in context.Parties
                         join o in context.Orgnizations on p.Id equals o.Id
                         where o.OrganizationType == OrganizationType.ATM
                         && vm.RegionId.GetValueOrDefault() == p.RegionId
                         && (vm.SubRegionId == null || vm.SubRegionId == p.SubregionId)
                         && (vm.StationId == null || vm.StationId == p.StationId)
                         select new
                         {
                             p.Id,
                             p.FormalName,
                             p.ShortName,
                             p.Address,
                             p.StationId
                         });
            var totalRows = query.Count();

            if (vm.StationId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.StationId == vm.StationId);
            }

            var items = await query
              .Select(x => new ATMCustodiansListViewModel()
              {
                  ATMId = x.Id,
                  Name = x.FormalName,
                  Address = x.Address
              })
              .Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();
            return new IndexViewModel<ATMCustodiansListViewModel>(items, totalRows);
        }
        [HttpGet]
        public Task<ATMCustodiansViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        public Task<int> PostAsync(ATMCustodiansViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
