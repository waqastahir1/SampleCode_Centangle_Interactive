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

using SOS.OrderTracking.Web.Shared.ViewModels.Vehicles;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [Authorize]
    [ApiController]
    public class VehiclesController : ControllerBase , IVehicleService
    {
        protected readonly AppDbContext context;
        private readonly ILogger<VehiclesController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmployeeService employeeService;
        public VehiclesController(AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<VehiclesController> logger, EmployeeService employeeService)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
            this.employeeService = employeeService;
        }

        public Task<VehiclesFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IndexViewModel<VehiclesListViewModel>> GetPageAsync([FromQuery]VehiclesAdditionalValueViewModel vm)
        {
            var query = (from v in context.Assets
                         where v.AssetType == AssetType.Vehicle
                         from a in context.AssetAllocations.Where(x => x.AssetId == v.Id && (!x.AllocatedThru.HasValue || x.AllocatedThru.Value >= MyDateTime.Now)).DefaultIfEmpty()
                         select new
                         {
                             v.Description,
                             v.RegionId,
                             v.SubregionId,
                             v.StationId,
                             a.PartyId
                         });

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
                query = query.Where(x => x.StationId == vm.StationId);// || x.StationId == 0  );
            }




            //Dictionary<int, string> dict = new Dictionary<int, string>();
            //foreach (var user in query)
            //{
            //    var relation = await employeeService.RelationshipDetail(user.PartyId, MyDateTime.Now);

            //    if (relation == null)
            //    {
            //        relation = new RelationshipDetailViewModel();
            //    }
            //    if (!dict.ContainsKey(user.Id))
            //    {
            //        dict.Add(user.Id, relation.OrganizationName);
            //    }
            //}
            var totalRows = query.Count();

            var items = await query
                .Select(x => new VehiclesListViewModel()
                {
                    VehicleDescription = x.Description,
                    Region = context.Parties.FirstOrDefault(y => y.Id == x.RegionId).FormalName,
                    SubRegion = context.Parties.FirstOrDefault(y => y.Id == x.SubregionId).FormalName,
                    Station = context.Parties.FirstOrDefault(y => y.Id == x.StationId).FormalName,
                    CrewOrVaultName = context.Parties.FirstOrDefault(y => y.Id == x.PartyId).FormalName
                })
                .Skip((vm.CurrentIndex -1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();
            if (!string.IsNullOrEmpty(vm.SearchKey))
            {
                items = items.Where(x => x.VehicleDescription.ToLower().Contains(vm.SearchKey.ToLower())
                || (!string.IsNullOrEmpty(x.CrewOrVaultName) && x.CrewOrVaultName.ToLower().Contains(vm.SearchKey.ToLower()))
                || (!string.IsNullOrEmpty(x.Station) && x.Station.ToLower().Contains(vm.SearchKey.ToLower()))).ToArray();
            }
            //foreach (var item in items)
            //{
            //    item.OrganizationName = dict.FirstOrDefault(x => x.Key == item.UserId).Value;
            //    if (string.IsNullOrEmpty(item.UserName))
            //        item.TickColor = "orange";
            //    else
            //        item.TickColor = "green";
            //}

            return new IndexViewModel<VehiclesListViewModel>(items, totalRows);
        }

        public Task<int> PostAsync(VehiclesFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
