using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class RegionalHeadController : ControllerBase ,IRegionalHeadService
    {
        private readonly AppDbContext context;

        private readonly ILogger<RegionalHeadController> logger;
        private readonly SequenceService sequenceService;
        private readonly EmployeeService peopleService;
        public UserManager<ApplicationUser> userManager { get; set; }

        public RegionalHeadController(AppDbContext appDbContext, ILogger<RegionalHeadController> logger,
            SequenceService sequenceService, UserManager<ApplicationUser> userManager, EmployeeService peopleService)
        {
            context = appDbContext;
            this.sequenceService = sequenceService;
            this.userManager = userManager;
            this.peopleService = peopleService;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IndexViewModel<RegionalHeadListViewModel>> GetPageAsync([FromQuery] AppointHeadsAdditionalValueViewModel vm)
        {

            var query = await(from r in context.PartyRelationships
                              join p in context.Parties on r.FromPartyId equals p.Id
                              where r.FromPartyRole == RoleType.RegionalHead
                              from pr in context.Parties
                              join o in context.Orgnizations on pr.Id equals o.Id
                              where r.ToPartyRole == RoleType.RegionalOrg && r.ToPartyId == pr.Id
                              select new RegionalHeadListViewModel()
                              {
                                  RegionId = r.ToPartyId,
                                  RegionName = pr.FormalName,
                                  EmployeeCode = p.ShortName,
                                  EmployeeName = p.FormalName,
                                  EmployeeId = p.Id,
                                  StartDate = r.StartDate,
                                  EndDate = r.ThruDate,
                                  Id = r.Id,
                                  IsActive = r.IsActive
                              }).ToListAsync();


            if (vm.RegionId.GetValueOrDefault() > 0)
            {
                query = query.Where(x => x.RegionId == vm.RegionId).ToList();//|| !x.RegionId.HasValue
            }
            if (!string.IsNullOrEmpty(vm.SearchKey))
            {
                query = query.Where(x => x.EmployeeName.ToLower().Contains(vm.SearchKey.ToLower())).ToList();
            }

            var totalRows = query.Count();
            var items = query.Skip((vm.CurrentIndex - 1 ) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArray();

            return new IndexViewModel<RegionalHeadListViewModel>(items, totalRows);
        }


        [HttpGet]
        public async Task<RegionalHeadViewModel> GetAsync(int id)
        {
            var member = await (from r in context.PartyRelationships
                                join p in context.Parties on r.FromPartyId equals p.Id
                                where r.Id == id
                                select new RegionalHeadViewModel()
                                {
                                    RegionName = context.Parties.Where(x => x.Id == r.ToPartyId).Select(x => x.FormalName).FirstOrDefault(),
                                    EmployeeName = p.FormalName,
                                    EmployeeId = p.Id,
                                    StartDate = r.StartDate,
                                    EndDate = r.ThruDate,
                                    Id = r.Id,
                                    RegionId = r.ToPartyId
                                }).FirstAsync();

            return member;
        }


        [HttpPost]
        public async Task<int> PostAsync(RegionalHeadViewModel SelectedItem)
        {
            try
            {

                PartyRelationship relationship = null;

                relationship = context.PartyRelationships.FirstOrDefault(x => x.Id == SelectedItem.Id && (!x.ThruDate.HasValue || x.ThruDate >= SelectedItem.StartDate));

                if (relationship == null)
                {
                    try
                    {
                        await peopleService.ChangeEmployeeRole(SelectedItem.EmployeeId, SelectedItem.RegionId,
                            SelectedItem.StartDate, SelectedItem.EndDate, true);
                    }
                    catch (Exception ex)
                    {
                        throw new BadRequestException(ex.Message);
                    }
                    relationship = new PartyRelationship();
                    relationship.Id = sequenceService.GetNextPartiesSequence();
                    relationship.ToPartyRole = RoleType.RegionalOrg;
                    relationship.ToPartyId = SelectedItem.RegionId;
                    relationship.FromPartyId = SelectedItem.EmployeeId;
                    relationship.FromPartyRole = RoleType.RegionalHead;
                    relationship.StartDate = SelectedItem.StartDate.GetValueOrDefault();
                    relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                    context.PartyRelationships.Add(relationship);

                    await context.SaveChangesAsync();

                    var employeeDetail = await context.Parties.FirstOrDefaultAsync(x => x.Id == SelectedItem.EmployeeId);
                    employeeDetail.RegionId = SelectedItem.RegionId;
                    context.Parties.Update(employeeDetail);
                    await context.SaveChangesAsync();

                }
                relationship.ThruDate = SelectedItem.EndDate;

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
                var parties = await(from pr in context.Parties
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
