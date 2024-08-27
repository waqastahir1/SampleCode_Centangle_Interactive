using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BoldReports.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ManageGaurdsController : ControllerBase, IManageGaurdsService
    {
        private readonly AppDbContext context;
        private readonly ILogger<ManageGaurdsController> logger;
        private readonly PartiesService partiesService;
        private readonly SequenceService sequenceService;
        public ManageGaurdsController(AppDbContext appDbContext,
            ILogger<ManageGaurdsController> logger, PartiesService partiesService, SequenceService sequenceService)
        {
            context = appDbContext;
            this.logger = logger;
            this.partiesService = partiesService;
            this.sequenceService = sequenceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllGaurds()
        {
            try
            {
                var AllGaurds = await (from r in context.Parties
                                       join p in context.People on r.Id equals p.Id
                                       where (p.EmploymentType == EmploymentType.Gaurd)
                                       select new SelectListItem(r.Id, r.FormalName))
                             .ToListAsync();
                return Ok(AllGaurds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        //get all banks to show in top dropdown
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var banks = await (from o in context.Orgnizations
                                   join p in context.Parties on o.Id equals p.Id
                                   where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                                   select new SelectListItem(p.Id, p.FormalName))
                                   .ToListAsync();

                return Ok(banks);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }
        //get selected gaurd's data by clicking on del button inside table
        //[HttpGet]
        //public async Task<IActionResult> Get(int id)
        //{
        //    try
        //    {

        //        var query = await (from r in context.PartyRelationships
        //                           where r.Id == id //&& r.ToPartyId != 1
        //                           select new GaurdsAllocationFormViewModel()
        //                           {
        //                               Id = r.Id,
        //                               GaurdId = r.FromPartyId,
        //                               OrganizationId = r.ToPartyId,
        //                           }).FirstOrDefaultAsync();

        //        return Ok(query);

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.ToString());
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> Post(GaurdsAllocationFormViewModel SelectedItem)
        //{
        //    try
        //    {
        //        PartyRelationship relationship = new PartyRelationship
        //        {
        //            Id = context.Sequences.GetNextPartiesSequence(),

        //        };
        //        relationship.FromPartyId = SelectedItem.GaurdId;
        //        //     relationship.StartDate = (DateTime)SelectedItem.StartDate;
        //        relationship.ToPartyId = SelectedItem.OrganizationId; // Parent Customer Id
        //        relationship.FromPartyRole = RoleType.Gaurd;
        //        relationship.ToPartyRole = RoleType.Customer;


        //        if (relationship.ThruDate.HasValue)
        //        {
        //            return BadRequest("This gaurd is already terminated!");
        //        }
        //        //if (SelectedItem.ThruDate.HasValue)
        //        //{
        //        //    if (SelectedItem.StartDate > SelectedItem.ThruDate)
        //        //    {
        //        //        return BadRequest("End date should be greater then start date!");
        //        //    }
        //        //    else
        //        //    {
        //        //        relationship.ThruDate = SelectedItem.ThruDate;
        //        //    }
        //        //}
        //        await context.SaveChangesAsync();

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.ToString());
        //        return BadRequest(ex.Message);
        //    }
        //}
        [HttpGet]
        public async Task<IndexViewModel<GaurdsAllocationListViewModel>> GetPageAsync([FromQuery] GaurdsAllocationAdditionalValueViewModel vm)
        {

            try
            {
                var query = (from o in context.Orgnizations
                             join p in context.Parties on o.Id equals p.Id
                             join r in context.PartyRelationships on o.Id equals r.FromPartyId
                             where o.OrganizationType.HasFlag(OrganizationType.ExternalOrganization)
                             // where r.ToPartyId == vm.MainCustomerId
                             && (vm.RegionId == null || p.RegionId == vm.RegionId || p.RegionId == null)
                             && (vm.SubRegionId == null || p.SubregionId == vm.SubRegionId || p.SubregionId == null)
                             && (vm.StationId == null || p.StationId == vm.StationId || p.StationId == null)
                             select new GaurdsAllocationListViewModel()
                             {
                                 Id = o.Id,
                                 BranchName = p.FormalName,
                                 BranchCode = p.ShortName,
                                 ContactNo = p.PersonalContactNo,
                                 Address = p.Address,
                                 ToPartyId = r.ToPartyId,
                                 //ActiveGaurdsCount = context.PartyRelationships.Where(x => x.FromPartyRole == RoleType.Gaurd
                                 //&& x.ToPartyId == r.FromPartyId
                                 //&& x.IsActive).Count(),
                                 ActiveGaurdsCount = context.GaurdingOrganizations.Where(x => x.BranchId == o.Id).FirstOrDefault().NoOfGaurdsAppointed,
                                 TotalGaurdsRequired = context.GaurdingOrganizations.Where(x => x.BranchId == o.Id).FirstOrDefault().TotalNoOfGaurdsRequired
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

                return new IndexViewModel<GaurdsAllocationListViewModel>(items, totalRows);
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        [HttpGet]
        public async Task<GaurdsAllocationFormViewModel> GetAsync(int id)
        {
            GaurdsAllocationFormViewModel gaurdsAllocationFormViewModel = null;
            try
            {
                gaurdsAllocationFormViewModel = await (from g in context.GaurdingOrganizations
                                                       where g.BranchId == id
                                                       select new GaurdsAllocationFormViewModel()
                                                       {
                                                           Id = g.Id,
                                                           NoOfShifts = g.NoOfShifts,
                                                           NoOfGaurds = g.TotalNoOfGaurdsRequired,
                                                           BranchId = id
                                                       }).FirstOrDefaultAsync();

                return gaurdsAllocationFormViewModel ?? new() { BranchId = id };
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

        }

        [HttpPost]
        public async Task<int> PostAsync(GaurdsAllocationFormViewModel selectedItem)
        {
            try
            {
                if (selectedItem.NoOfShifts == 0)
                    throw new BadRequestException("Please select no of shifts");
                GaurdingOrganization gaurdingOrganization = null;
                gaurdingOrganization = await context.GaurdingOrganizations.FirstOrDefaultAsync(x => x.Id == selectedItem.Id);
                if (gaurdingOrganization == null)
                {
                    gaurdingOrganization = new GaurdingOrganization()
                    {
                        Id = sequenceService.GetNextCommonSequence(),
                        BranchId = selectedItem.BranchId
                    };

                    context.GaurdingOrganizations.Add(gaurdingOrganization);
                }
                if (gaurdingOrganization.NoOfGaurdsAppointed > selectedItem.NoOfGaurds)
                    throw new BadRequestException($"{gaurdingOrganization.NoOfGaurdsAppointed} gaurds" +
                        $" are appointed please terminate someone first before contracting gaurds capacity!");

                gaurdingOrganization.NoOfShifts = selectedItem.NoOfShifts;
                gaurdingOrganization.TotalNoOfGaurdsRequired = selectedItem.NoOfGaurds;
                // gaurdingOrganization.BranchId = selectedItem.BranchId;


                await context.SaveChangesAsync();
                return gaurdingOrganization.Id;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
