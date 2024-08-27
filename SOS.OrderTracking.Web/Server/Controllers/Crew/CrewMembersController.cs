using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class CrewMembersController : ControllerBase, ICrewMembersService
    {
        private readonly AppDbContext context;


        private readonly SequenceService sequenceService;
        private readonly EmployeeService employeeService;
        public UserManager<ApplicationUser> userManager { get; set; }

        public CrewMembersController(AppDbContext appDbContext,
            SequenceService sequenceService, UserManager<ApplicationUser> userManager, EmployeeService peopleService)
        {
            context = appDbContext;
            this.sequenceService = sequenceService;
            this.userManager = userManager;
            this.employeeService = peopleService;
        }

        [HttpGet]
        public async Task<IndexViewModel<CrewMemberListModel>> GetPageAsync([FromQuery] CrewMemberAdditionalValueViewModel vm)
        {
            if (vm.CrewId == 0)
                throw new BadRequestException("Please select Crew"); //NoContent();

            var query = (from r in context.PartyRelationships
                         join p in context.Parties on r.FromPartyId equals p.Id
                         join e in context.People on p.Id equals e.Id
                         where r.ToPartyId == vm.CrewId
                         select new CrewMemberListModel()
                         {
                             CrewId = r.ToPartyId,
                             RelationshipType = r.FromPartyRole,
                             EmployeeName = p.FormalName,
                             EmployeeCode = p.ShortName,
                             EmployeeId = p.Id,
                             ImageLink = p.ImageLink,
                             StartDate = r.StartDate,
                             EndDate = r.ThruDate,
                             Id = r.Id,
                             IsActive = r.IsActive,
                             NationalId = e.NationalId,

                             CheckinTime = context.EmployeeAttendance.FirstOrDefault(x => x.RelationshipId == r.Id
                             && r.IsActive && (x.AttendanceDate.Date.Equals(MyDateTime.Now.Date) && x.AttendanceState == AttendanceState.Present)).CheckinTime,

                             CheckoutTime = context.EmployeeAttendance.FirstOrDefault(x => x.RelationshipId == r.Id
                             && r.IsActive && (x.AttendanceDate.Date.Equals(MyDateTime.Now.Date) && x.AttendanceState == AttendanceState.Present)).CheckoutTime
                         });

            if (vm.OnlyActive)
            {
                query = query.Where(x => x.IsActive);
            }
            if (!string.IsNullOrEmpty(vm.SortColumn))
            {
                if (vm.SortColumn.EndsWith("True"))
                {
                    query = query.OrderBy(vm.SortColumn.Split('-').First());
                }
                else
                {
                    query = query.OrderByDescending(vm.SortColumn.Split('-').First());
                }

            }
            var totalRows = query.Count();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<CrewMemberListModel>(items, totalRows);
        }
        [HttpGet]
        public async Task<CrewMemberViewModel> GetAsync(int id)
        {
            var member = await (from r in context.PartyRelationships
                                join p in context.Parties on r.FromPartyId equals p.Id
                                where r.Id == id
                                select new CrewMemberViewModel()
                                {

                                    RelationshipType = r.FromPartyRole,
                                    EmployeeName = p.FormalName,
                                    EmployeeId = p.Id,
                                    StartDate = r.StartDate,
                                    EndDate = r.ThruDate,
                                    Id = r.Id,
                                    CrewId = r.ToPartyId
                                }).FirstAsync();

            return member;
        }
        [HttpPost]
        public async Task<int> PostAsync(CrewMemberViewModel SelectedItem)
        {
            try
            {
                PartyRelationship relationship = null;
                relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == SelectedItem.CrewId && x.FromPartyRole == RoleType.Crew);
                if (SelectedItem.Id == 0)
                {
                    if (SelectedItem.StartDate < relationship.StartDate)
                    {
                        //if selected employee startdate is less then crew creation date
                        throw new BadRequestException("Crew creation date is " + relationship.StartDate.ToString("dd-MM-yyyy") +
                            " please add start date of crew member greater then crew creation date!");
                    }
                    // if end date is less then start date
                    if (SelectedItem.EndDate.HasValue)
                    {
                        if (SelectedItem.EndDate < SelectedItem.StartDate)
                        {
                            throw new BadRequestException("End Date should be greater then or equal to start date");
                        }
                    }
                    //if selected role is already exist in crew
                    relationship = await context.PartyRelationships.FirstOrDefaultAsync(
                      x => x.ToPartyId == SelectedItem.CrewId
                      && x.FromPartyRole == SelectedItem.RelationshipType
                      && (x.ThruDate == null || x.ThruDate >= SelectedItem.StartDate));

                    if (relationship != null)
                    {
                        var NameOfEmployee = await context.Parties.FirstOrDefaultAsync(x => x.Id == relationship.FromPartyId);// && relationship.StartDate <= SelectedItem.StartDate);
                        throw new BadRequestException(SelectedItem.RelationshipType
                                 + " with same date is already exist with Name : " + NameOfEmployee.FormalName
                                 + " please add/change end date of " + NameOfEmployee.FormalName
                                 + " if you want to add new "
                                 + SelectedItem.RelationshipType);
                    }
                    //if selected crew member is already exist in crew
                    relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == SelectedItem.EmployeeId
                        && x.ToPartyId == SelectedItem.CrewId && (!x.ThruDate.HasValue || x.ThruDate > SelectedItem.StartDate));
                    if (relationship != null)
                    {
                        throw new BadRequestException("This crew Member is already exist if you want it to another role then please terminate it first by adding end date!");
                    }
                    //if selected person is already exist in any other organization in selected date
                    relationship = context.PartyRelationships.Where(x => x.FromPartyId == SelectedItem.EmployeeId
                     && x.ToPartyId != 1
                     && (!x.ThruDate.HasValue || x.ThruDate.Value.Date >= SelectedItem.StartDate.GetValueOrDefault().Date))
                        .OrderByDescending(x=> x.StartDate).FirstOrDefault();
                    if (relationship != default)
                    {
                        if (relationship.StartDate.Date > SelectedItem.StartDate.GetValueOrDefault().Date)
                            throw new BadRequestException($"please select start date after {relationship.StartDate.ToString("dd-MM-yyyy")}");

                        relationship.ThruDate = SelectedItem.StartDate;
                        relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                        await context.SaveChangesAsync();
                    }
                }

                relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                if (relationship == null)
                {
                    try
                    {
                        //await employeeService.ChangeEmployeeRole(SelectedItem.EmployeeId, SelectedItem.CrewId,
                        //SelectedItem.StartDate, SelectedItem.EndDate, SelectedItem.RelationshipType == RoleType.CheifCrewAgent);
                    }
                    catch (Exception ex)
                    {
                        throw new BadRequestException(ex.Message);
                    }

                    relationship = new PartyRelationship();
                    relationship.Id = sequenceService.GetNextPartiesSequence();
                    relationship.ToPartyRole = RoleType.Crew;
                    relationship.ToPartyId = SelectedItem.CrewId;
                    relationship.FromPartyId = SelectedItem.EmployeeId;
                    relationship.FromPartyRole = SelectedItem.RelationshipType;
                    relationship.StartDate = SelectedItem.StartDate.GetValueOrDefault();
                    context.PartyRelationships.Add(relationship);
                }
                relationship.ThruDate = SelectedItem.EndDate;
                relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                await context.SaveChangesAsync();

                return relationship.Id;

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        [HttpPost]
        public async Task<int> RemoveCrewMember(CrewMemberOperationsViewModel SelectedCrewMember)
        {
            PartyRelationship relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.Id == SelectedCrewMember.RelationshipId && x.ToPartyId == SelectedCrewMember.CrewId);
            if (relationship != null)
            {
                if (relationship.StartDate.Date > MyDateTime.Now.Date)
                {
                    throw new BadRequestException("You cannot remove CrewMember in previous date!");
                }
                else
                {
                    context.PartyRelationships.Remove(relationship);
                    await context.SaveChangesAsync();
                }
            }
            return relationship.Id;
        }

        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetPotentialCrewMembers(int regionId, int? subRegionId, int? stationId)
        {
            var parties = await (from pr in context.Parties
                                 from p in context.People.Where(x => x.Id == pr.Id)
                                 where p.EmploymentType == EmploymentType.Gaurd
                                 && p.Status
                                 && pr.RegionId == regionId
                                 && (subRegionId == null || pr.SubregionId == subRegionId)
                                 && (stationId == null || pr.StationId == stationId)
                                 orderby pr.FormalName
                                 select new
                                 {
                                     IntValue = pr.Id,
                                     pr.ShortName,
                                     pr.FormalName,
                                     p.NationalId
                                 }).ToListAsync();
            return parties.Select(x => new SelectListItem(x.IntValue, x.ShortName + "-" + x.FormalName + " " + x.NationalId));
        }

        [HttpGet]
        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {
            RelationshipDetailViewModel relationshipDetailViewModel = null;
            PartyRelationship relationship = null;
            //terminating employee from another organization
            relationship = context.PartyRelationships.Where(x => x.FromPartyId == employeeId
                && x.ToPartyId != 1
                && (!x.ThruDate.HasValue || x.ThruDate.Value >= startDate)).OrderByDescending(x=> x.StartDate).FirstOrDefault();
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
        public async Task<CrewMemberOperationsViewModel> GetMemberDetail(int id)
        {
            var member = from p in context.Parties
                         join r in context.PartyRelationships on p.Id equals r.FromPartyId
                         where r.Id == id && r.ToPartyRole == RoleType.Crew
                         select new CrewMemberOperationsViewModel()
                         {
                             RelationshipId = r.Id,
                             EmployeeName = p.FormalName,
                             StartDate = r.StartDate,
                             EndDate = r.ThruDate,
                             EmployeeId = p.Id,
                             CrewId = r.ToPartyId
                         };
            return await member.FirstOrDefaultAsync();
        }


    }
}
