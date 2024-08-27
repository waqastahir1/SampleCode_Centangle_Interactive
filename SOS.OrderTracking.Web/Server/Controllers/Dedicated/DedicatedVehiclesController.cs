using IdentityServer4.Events;
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
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Admin
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class DedicatedVehiclesController : ControllerBase, IBranchVehicleService
    {
        private readonly AppDbContext context;
        private readonly ILogger<DedicatedVehiclesController> _logger;
        private readonly PartiesService _partiesService;
        private readonly AssetsService _assetsService;
        private readonly SequenceService _sequenceService;

        public DedicatedVehiclesController(AppDbContext context, ILogger<DedicatedVehiclesController> logger, PartiesService partiesService,
            AssetsService assetsService, SequenceService sequenceService)
        {
            this.context = context;
            _logger = logger;
            _partiesService = partiesService;
            _assetsService = assetsService;
            _sequenceService = sequenceService;
        }
        [HttpGet]
        public async Task<VehicleToBranchFormViewModel> GetAsync(int id)
        {
            var query = await (from aa in context.AssetAllocations
                               join aset in context.Assets on aa.AssetId equals aset.Id
                               where aa.Id == id
                               select new VehicleToBranchFormViewModel()
                               {
                                   AssetAllocationId = aa.Id,
                                   PartyId = aa.PartyId,
                                   AssetId = aa.AssetId,
                                   Vehicle = aset.Description,
                                   StartDate = aa.AllocatedFrom,
                                   EndDate = aa.AllocatedThru,
                                   AllocatedBy = aa.AllocatedBy,
                                   AllocatedAt = aa.AllocatedAt
                               }).FirstOrDefaultAsync();
            return query;
        }
        [HttpGet]
        public async Task<IndexViewModel<BranchVehicleListViewModel>> GetPageAsync([FromQuery] BranchToVehicleInputViewModel vm)
        {
             
            var query = from a in context.AssetAllocations
                        join aset in context.Assets on a.AssetId equals aset.Id
                        join p in context.Parties on a.PartyId equals p.Id
                        where p.Id == vm.PartyId
                        orderby a.AllocatedFrom descending
                        select new BranchVehicleListViewModel()
                        {
                            IsActive = ((a.AllocatedThru == null || a.AllocatedThru >= MyDateTime.Now) && a.AllocatedFrom <= MyDateTime.Now),
                            AllocationId = a.Id,
                            Asset = aset.Description,
                            FormalName = p.FormalName,
                            AllocatedFrom = a.AllocatedFrom,
                            AllocatedThru = a.AllocatedThru,
                            AllocatedBy = a.AllocatedBy
                        };

            if (!string.IsNullOrEmpty(vm.SortColumn))
                query = query.OrderBy(vm.SortColumn);

            var totalRows = await query.CountAsync();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<BranchVehicleListViewModel>(items, totalRows);
        }
        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetBranchAsset(int regionId, int subRegionId, int stationId)
        {
            var query = from a in context.Assets
                        where (a.RegionId == regionId)
                        && (a.SubregionId == subRegionId)
                        && (a.StationId == stationId)
                        select new SelectListItem()
                        {
                            IntValue = a.Id,
                            Text = a.Description
                        };

            var assets = await query.ToArrayAsync();
            return assets;
        }
        [HttpGet]
        public async Task<VehicleRemoveFormViewModel> GetVehicleDetails(int Id)
        {
            if (Id == 0)
            {
                return new VehicleRemoveFormViewModel();
            }
            var query = from aa in context.AssetAllocations
                        join aset in context.Assets on aa.AssetId equals aset.Id
                        where aa.Id == Id
                        select new VehicleRemoveFormViewModel()
                        {
                            Id = aa.Id,
                            AllocatedFrom = aa.AllocatedFrom,
                            AllocatedThru = aa.AllocatedThru,
                            AssetId = aa.AssetId,
                            PartyId = aa.PartyId,
                            AllocatedBy = aa.AllocatedBy,
                            AllocatedAt = aa.AllocatedAt,
                            Vehicle = aset.Description
                        };
            return await query.FirstOrDefaultAsync();
        }
        public async Task<int> RemoveVehicle(VehicleRemoveFormViewModel model)
        {
            AssetAllocation assetAllocation = new AssetAllocation()
            {
                Id = model.Id,
                AssetId = model.AssetId,
                PartyId = model.PartyId,
                AllocatedFrom = model.AllocatedFrom,
                AllocatedThru = model.AllocatedThru,
                AllocatedBy = model.AllocatedBy,
                AllocatedAt = model.AllocatedAt
            };
            context.AssetAllocations.Remove(assetAllocation);
            await context.SaveChangesAsync();
            return assetAllocation.Id;
        }
        public async Task<int> PostAsync(VehicleToBranchFormViewModel selectedItem)
        {
            var dedicatedVehicle = await context.DedicatedVehiclesCapacities.FirstOrDefaultAsync(x => x.OrganizationId == selectedItem.PartyId);
            if(dedicatedVehicle == null || (dedicatedVehicle != null && dedicatedVehicle.VehicleCapacity == 0))
            {
                throw new BadRequestException("Please specify number of dedicated vehicles first in Left page");
            }

            AssetAllocation assetAllocation = null;
            if (selectedItem.EndDate.HasValue)
            {
                if (selectedItem.EndDate < selectedItem.StartDate)
                {
                    throw new BadRequestException("End Date should be greater then or equal to start date");
                }
            }
            if (selectedItem.AssetAllocationId == 0)
            {
                //var alreadyExist = await context.AssetAllocations.FirstOrDefaultAsync(x => x.AssetId == selectedItem.AssetId
                //                                    // && x.PartyId == selectedItem.PartyId
                //                                    && (x.AllocatedThru == null || (x.AllocatedThru > selectedItem.StartDate && x.AllocatedFrom < selectedItem.StartDate)));

                var activeVehiclesCount = await context.AssetAllocations.Where(x => x.PartyId == selectedItem.PartyId && (x.AllocatedThru == null || x.AllocatedThru >= DateTime.Now)).CountAsync();

                if (activeVehiclesCount == dedicatedVehicle.VehicleCapacity)
                    throw new BadRequestException("Number of dedicated vehicle specified for this branch is full if you want to add another vehicle then please update number of dedicated vehicles");

                //if (dedicatedVehicle.FromDate >= selectedItem.StartDate)
                //    throw new BadRequestException($"Start date should be greater then start date of dedicated vehicle allocation : {dedicatedVehicle.FromDate.ToString("dd-MM-yyyy")}");

                var alreadyExist = await (from a in context.AssetAllocations
                                          join p in context.Parties on a.PartyId equals p.Id
                                          where p.Orgnization.OrganizationType.HasFlag(OrganizationType.CustomerBranch)
                                          && (a.AllocatedThru == null || (a.AllocatedThru > selectedItem.StartDate && a.AllocatedFrom < selectedItem.StartDate))
                                          && a.AssetId == selectedItem.AssetId
                                          select a).FirstOrDefaultAsync();

                if (alreadyExist != null)
                {
                    var vehicleNumber = await context.Assets.Where(x => x.Id == alreadyExist.AssetId).Select(x => x.Description).FirstOrDefaultAsync();
                    var branchName = await context.Parties.Where(x => x.Id == alreadyExist.PartyId).Select(x => x.FormalName).FirstOrDefaultAsync();

                    throw new BadRequestException($"{vehicleNumber} is already allocated to {branchName} in selected date. Please enter End Date to re-allocate vehicle.");
                }

            }
            #region previous work commented
            //else if (await ReallocateValidation(selectedItem))
            //{
            //    #region Finding Vehicle And Branch Name
            //    var vehicleNumber = await context.Assets.Where(x => x.Id == selectedItem.AssetId).Select(x => x.Description).FirstOrDefaultAsync();
            //    var branchName = await context.Parties.Where(x => x.Id == selectedItem.PartyId).Select(x => x.FormalName).FirstOrDefaultAsync();
            //    #endregion
            //    throw new BadRequestException($"{vehicleNumber} is already allocated to {branchName}. Please enter End Date to re-allocate vehicle.");
            //}
            //else if (await UsedDateValidation(selectedItem))
            //{
            //    throw new BadRequestException($"You have already used this date {selectedItem.StartDate.Value.ToString("dd-MM-yyyy")}.");
            //}
            //return await SaveOrUpdate(selectedItem);
            #endregion


            assetAllocation = await context.AssetAllocations.FirstOrDefaultAsync(x => x.Id == selectedItem.AssetAllocationId);
            if (assetAllocation == null)
            {
                assetAllocation = new AssetAllocation
                {
                    Id = _sequenceService.GetNextCommonSequence(),
                    PartyId = selectedItem.PartyId,
                    AssetId = selectedItem.AssetId.GetValueOrDefault(),
                    AllocatedFrom = selectedItem.StartDate.GetValueOrDefault()
                };
                context.AssetAllocations.Add(assetAllocation);
            }

            assetAllocation.AllocatedThru = selectedItem.EndDate;


            await context.SaveChangesAsync();

            return assetAllocation.Id;
        }
        #region Valiations on Server Side commented
        //private async Task<int> SaveOrUpdate(VehicleToBranchFormViewModel selectedItem)
        //{
        //    AssetAllocation assetAllocation = new AssetAllocation();
        //    assetAllocation.PartyId = selectedItem.PartyId;
        //    assetAllocation.AssetId = selectedItem.AssetId;
        //    assetAllocation.AllocatedFrom = selectedItem.StartDate.Value;
        //    assetAllocation.AllocatedThru = (selectedItem.EndDate.HasValue == true ? selectedItem.EndDate : null);
        //    if (selectedItem.AssetAllocationId == 0)
        //    {
        //        assetAllocation.Id = _sequenceService.GetNextCommonSequence();
        //        context.AssetAllocations.Add(assetAllocation);
        //    }
        //    else
        //    {
        //        assetAllocation.Id = selectedItem.AssetAllocationId;
        //        context.AssetAllocations.Update(assetAllocation);
        //    }
        //    await context.SaveChangesAsync();
        //    return assetAllocation.Id;
        //}
        //private async Task<bool> UsedDateValidation(VehicleToBranchFormViewModel selectedItem)
        //{
        //    var query = await (from aa in context.AssetAllocations
        //                       where
        //                       aa.PartyId == selectedItem.PartyId
        //                       && aa.AssetId == selectedItem.AssetId
        //                       && aa.AllocatedFrom != DateTime.Parse("01/01/01 00:00")
        //                       && aa.AllocatedThru != null
        //                       && (selectedItem.StartDate.Value.Date <= aa.AllocatedFrom.Date
        //                        || selectedItem.StartDate.Value.Date <= aa.AllocatedThru.Value.Date)
        //                       select aa.Id)
        //                .AnyAsync();
        //    return query;
        //}
        //private async Task<bool> ReallocateValidation(VehicleToBranchFormViewModel selectedItem)
        //{
        //    return await context.AssetAllocations.AnyAsync(x => x.AssetId == selectedItem.AssetId
        //                                        && x.PartyId == selectedItem.PartyId
        //                                        && x.AllocatedThru == null
        //                                        && selectedItem.AssetAllocationId == 0);
        //}
        #endregion
    }
}
