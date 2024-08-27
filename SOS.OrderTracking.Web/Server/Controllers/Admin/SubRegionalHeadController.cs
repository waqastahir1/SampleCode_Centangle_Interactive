using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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


namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class SubRegionalHeadController : ControllerBase ,ISubRegionalHeadService
    {
        private readonly AppDbContext context;

        private readonly ILogger<SubRegionalHeadController> logger;
        private readonly SequenceService sequenceService;
        private readonly EmployeeService peopleService;
        private UserManager<ApplicationUser> userManager { get; set; }

        public SubRegionalHeadController(AppDbContext appDbContext, ILogger<SubRegionalHeadController> logger,
            SequenceService sequenceService, UserManager<ApplicationUser> userManager, EmployeeService peopleService)
        {
            context = appDbContext;
            this.sequenceService = sequenceService;
            this.userManager = userManager;
            this.peopleService = peopleService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IndexViewModel<SubRegionalHeadListViewModel>> GetPageAsync([FromQuery]AppointHeadsAdditionalValueViewModel vm)
        {
            var query = await(from r in context.PartyRelationships
                              join p in context.Parties on r.FromPartyId equals p.Id
                              where r.FromPartyRole == RoleType.SubRegionalHead
                              from pr in context.Parties
                              join o in context.Orgnizations on pr.Id equals o.Id
                              where r.ToPartyRole == RoleType.SubRegionalOrg && r.ToPartyId == pr.Id
                              select new SubRegionalHeadListViewModel()
                              {
                                  SubRegionId = r.ToPartyId,
                                  Code = p.ShortName,
                                  SubRegionName = pr.FormalName,
                                  Name = p.FormalName,
                                  RegionId = p.RegionId,
                                  EmployeeName = p.FormalName,
                                  EmployeeId = p.Id,
                                  RelationshipId = r.Id,
                                  StartDate = r.StartDate,
                                  EndDate = r.ThruDate,
                                  Id = r.Id,
                                  IsActive = r.IsActive
                              }).ToListAsync();


            if (vm.RegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.RegionId == vm.RegionId).ToList();
            }
            if (vm.SubRegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.SubRegionId == vm.SubRegionId).ToList();
            }
            if (!string.IsNullOrEmpty(vm.SearchKey))
            {
                query = query.Where(x => x.EmployeeName.ToLower().Contains(vm.SearchKey.ToLower())).ToList();
            }
            var totalRows = query.Count();

            var items = query.Skip((vm.CurrentIndex - 1 ) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArray();

            return new IndexViewModel<SubRegionalHeadListViewModel>(items, totalRows);
        }

        [HttpGet]
        public async Task<SubRegionalHeadViewModel> GetAsync(int id)
        {
            var member = await (from r in context.PartyRelationships
                                join p in context.Parties on r.FromPartyId equals p.Id
                                where r.Id == id
                                select new SubRegionalHeadViewModel()
                                {
                                    Name = p.FormalName,
                                    RegionName = context.Parties.Where(x => x.Id == r.ToPartyId).Select(x => x.FormalName).FirstOrDefault(),
                                    SubRegionName = context.Parties.Where(x => x.Id == r.ToPartyId).Select(x => x.FormalName).FirstOrDefault(),
                                    EmployeeId = p.Id,
                                    RelationshipId = r.Id,
                                    StartDate = r.StartDate,
                                    EndDate = r.ThruDate,
                                    Id = r.Id,
                                    SubRegionId = r.ToPartyId
                                }).FirstAsync();
            int? regionId = (await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == member.SubRegionId)).ToPartyId;
            member.RegionName = context.Parties.Where(x => x.Id == regionId).Select(x => x.FormalName).FirstOrDefault();

            return member;
        }
        [HttpPost]
        public async Task<int> PostAsync(SubRegionalHeadViewModel selectedItem)
        {
            try
            {
                PartyRelationship relationship = null;


                relationship = context.PartyRelationships.FirstOrDefault(x => x.Id == selectedItem.Id && (!x.ThruDate.HasValue || x.ThruDate >= selectedItem.StartDate));
                if (relationship == null)
                {
                    try
                    {
                        await peopleService.ChangeEmployeeRole(selectedItem.EmployeeId, selectedItem.SubRegionId,
                            selectedItem.StartDate, selectedItem.EndDate, true);
                    }
                    catch (Exception ex)
                    {
                        throw new BadRequestException(ex.Message);
                    }

                    relationship = new PartyRelationship();
                    relationship.Id = sequenceService.GetNextPartiesSequence();
                    relationship.ToPartyRole = RoleType.SubRegionalOrg;
                    relationship.ToPartyId = selectedItem.SubRegionId;
                    relationship.FromPartyId = selectedItem.EmployeeId;
                    relationship.FromPartyRole = RoleType.SubRegionalHead;
                    relationship.StartDate = selectedItem.StartDate.GetValueOrDefault();
                    relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                    context.PartyRelationships.Add(relationship);

                    int? regionId = (await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == selectedItem.SubRegionId)).ToPartyId;//getting regionId of selected Subregion
                    var employeeDetail = await context.Parties.FirstOrDefaultAsync(x => x.Id == selectedItem.EmployeeId);
                    employeeDetail.RegionId = regionId;
                    employeeDetail.SubregionId = selectedItem.SubRegionId;
                    context.Parties.Update(employeeDetail);
                    await context.SaveChangesAsync();

                }
                relationship.ThruDate = selectedItem.EndDate;

                await context.SaveChangesAsync();
                return relationship.Id;

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        [HttpGet]
        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {
            RelationshipDetailViewModel relationshipDetailViewModel = null;
            PartyRelationship relationship = null;
            //terminating employee from another organization
            relationship = context.PartyRelationships.Where(x => x.FromPartyId == employeeId
                && x.ToPartyId != 1
                && (!x.ThruDate.HasValue || x.ThruDate.Value >= startDate)).FirstOrDefault();
            if (relationship != null)
            {
                var detail = context.Parties.FirstOrDefault(x => x.Id == relationship.ToPartyId);
                OrganizationType organizationType = (await context.Orgnizations.FirstOrDefaultAsync(x => x.Id == relationship.ToPartyId)).OrganizationType;
                relationshipDetailViewModel = new RelationshipDetailViewModel();
                relationshipDetailViewModel.EmployeeId = detail.Id;
                relationshipDetailViewModel.OrganizationName = detail.ShortName + " - " + detail.FormalName;
                relationshipDetailViewModel.OrganizationTypeAsString = Enum.GetName(typeof(OrganizationType), organizationType);
                relationshipDetailViewModel.StartDate = relationship.StartDate;
                relationshipDetailViewModel.EndDate = relationship.ThruDate;
                return relationshipDetailViewModel;
            }
            return relationshipDetailViewModel;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetRegularEmployeesAsync()
        {
            try
            {
                var parties = await (from pr in context.Parties
                                     join p in context.People on pr.Id equals p.Id
                                     where p.EmploymentType == EmploymentType.Regular
                                     select new
                                     {
                                         pr.Id,
                                         pr.ShortName,
                                         pr.FormalName,
                                         p.NationalId
                                     }).ToArrayAsync();

                return parties.Select(x => new SelectListItem()
                {
                    IntValue = x.Id,
                    Text = $"{x.ShortName}-{x.FormalName} {x.NationalId}".Trim()
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw new BadRequestException(ex.Message);
            }
        }
    }
}
