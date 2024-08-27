using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Admin
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class DedicatedBranchesController : ControllerBase, IBranchService
    {
        private readonly AppDbContext context;
        private readonly ILogger<DedicatedBranchesController> _logger;
        private readonly PartiesService _partiesService;

        public DedicatedBranchesController(AppDbContext context, ILogger<DedicatedBranchesController> logger, PartiesService partiesService)
        {
            this.context = context;
            _logger = logger;
            _partiesService = partiesService;
        }
        [HttpGet]
        public async Task<BranchesFormViewModel> GetAsync(int id)
        {
            var query = await (from o in context.Orgnizations
                               from d in context.DedicatedVehiclesCapacities.Where(x=> x.OrganizationId == o.Id).DefaultIfEmpty()
                               join p in context.Parties on o.Id equals p.Id
                               where o.Id == id
                               select new BranchesFormViewModel()
                               {
                                   Id = o.Id,
                                   BranchName = p.FormalName,
                                   DedicatedVehicleCapacity = d.VehicleCapacity,
                                   StartDate = d.FromDate,
                                   EndDate = d.ToDate
                               }).FirstOrDefaultAsync();
            return query;
        }

        [HttpGet]
        public async Task<IndexViewModel<BranchesListViewModel>> GetPageAsync([FromQuery] BranchesAdditionalValueViewModel vm)
        {
            var query = (from o in context.Parties
                         from r in context.PartyRelationships.Where(x => x.FromPartyId == o.Id && x.ToPartyId > 0).DefaultIfEmpty()
                         from d in context.DedicatedVehiclesCapacities.Where(x=>x.OrganizationId == o.Id)
                         where (vm.RegionId == null || o.RegionId == vm.RegionId || o.RegionId == null)
                         && (vm.SubRegionId == null || o.SubregionId == vm.SubRegionId || o.SubregionId == null)
                         && (vm.StationId == null || o.StationId == vm.StationId || o.StationId == null)
                         && d.IsActive
                         select new BranchesListViewModel()
                         {
                             Id = o.Id,
                             ToPartyId = r.ToPartyId,
                             BranchCode = o.ShortName,
                             BranchName = o.FormalName,
                             RegionName = context.Parties.FirstOrDefault(x => x.Id == o.RegionId).FormalName,
                             SubRegionName = context.Parties.FirstOrDefault(x => x.Id == o.SubregionId).FormalName,
                             StationName = context.Parties.FirstOrDefault(x => x.Id == o.StationId).FormalName,
                             RegionId = o.RegionId,
                             SubRegionId = o.SubregionId,
                             StationId = o.StationId,
                             DedicatedVehicleCapacity = d.VehicleCapacity,
                             ActiveVehiclesCount = context.AssetAllocations.Where(x => x.PartyId == o.Id && x.AllocatedFrom <= MyDateTime.Now  && (x.AllocatedThru == null || x.AllocatedThru >= MyDateTime.Now)).Count()
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

            return new IndexViewModel<BranchesListViewModel>(items, totalRows);
        }

        [HttpPost]
        public async Task<int> PostAsync(BranchesFormViewModel selectedItem)
        {
            //throw new InvalidOperationException("Dedicated vechile branches are synced from gbms and cannot be updated from cit portal");
            var organization = await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == selectedItem.Id);
            // Organization.DedicatedVehicleCapacity = selectedItem.DedicatedVehicleCapacity;

            var dedicatedVehicle = await context.DedicatedVehiclesCapacities.FirstOrDefaultAsync(x => x.OrganizationId == organization.Id);
            if (dedicatedVehicle == null)
            {
                dedicatedVehicle = new DedicatedVehiclesCapacity()
                {
                    Id = (context.DedicatedVehiclesCapacities.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0) + 1,
                    OrganizationId = organization.Id,
                    FromDate = selectedItem.StartDate.GetValueOrDefault(),
                    CreatedAt = DateTime.Now,
                    CreatedBy = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value,
                };

                context.DedicatedVehiclesCapacities.Add(dedicatedVehicle);
            }
            dedicatedVehicle.VehicleCapacity = (byte)selectedItem.DedicatedVehicleCapacity;
            dedicatedVehicle.UpdatedAt = DateTime.Now;
            dedicatedVehicle.UpdatedBy = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            dedicatedVehicle.ToDate = selectedItem.EndDate;


            await context.SaveChangesAsync();
            return organization.Id;
        }
    }
}
