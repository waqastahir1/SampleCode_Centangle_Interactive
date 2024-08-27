using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Server.Controllers.Operations
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class AtmCustodianMembersController : ControllerBase, IATMCustodianMembersService
    {
        private readonly AppDbContext context;
        private readonly SequenceService sequenceService;
        private readonly UserManager<ApplicationUser> userManager;
        public AtmCustodianMembersController(AppDbContext context, SequenceService sequenceService, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.sequenceService = sequenceService;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<IndexViewModel<AtmCustodianMembersListViewModel>> GetPageAsync([FromQuery] AtmCustodianMembersAdditionalValueViewModel vm)
        {
            if (vm.ATMId == 0)
                throw new NotFoundException("Please select ATM"); //NoContent();

            var query = (from r in context.PartyRelationships
                         join p in context.Parties on r.FromPartyId equals p.Id
                         join e in context.People on p.Id equals e.Id
                         where r.ToPartyId == vm.ATMId
                         select new AtmCustodianMembersListViewModel()
                         {
                             ATMId = r.ToPartyId,
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
                         });

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

            return new IndexViewModel<AtmCustodianMembersListViewModel>(items, totalRows);
        }
        [HttpGet]
        public async Task<AtmCustodianMembersFormViewModel> GetAsync(int id)
        {
            var member = await (from r in context.PartyRelationships
                                join p in context.Parties on r.FromPartyId equals p.Id
                                where r.Id == id
                                select new AtmCustodianMembersFormViewModel()
                                {

                                    PersonType = r.FromPartyRole == RoleType.ATMCashier ? 1 : 2,
                                    PersonId = p.Id,
                                    StartDate = r.StartDate,
                                    EndDate = r.ThruDate,
                                    RelationshipId = r.Id,
                                    ATMId = r.ToPartyId
                                }).FirstAsync();

            return member;
        }
        [HttpPost]
        public async Task<int> PostAsync(AtmCustodianMembersFormViewModel selectedItem)
        {
            try
            {
                PartyRelationship relationship = null;
                // if end date is less then start date
                if (selectedItem.EndDate.HasValue)
                {
                    if (selectedItem.EndDate < selectedItem.StartDate)
                    {
                        throw new BadRequestException("End Date should be greater then or equal to start date");
                    }
                }
                //if selected role is already exist in ATM
                relationship = await context.PartyRelationships.FirstOrDefaultAsync(
                  x => x.ToPartyId == selectedItem.ATMId
                  && x.FromPartyRole == (selectedItem.PersonType == 1 ? RoleType.ATMCashier : RoleType.ATMTechnician)
                  && x.ToPartyRole == RoleType.ATM
                  && x.IsActive);

                if (relationship != null)
                {
                    var NameOfEmployee = await context.Parties.FirstOrDefaultAsync(x => x.Id == relationship.FromPartyId && relationship.StartDate <= selectedItem.StartDate);
                    var roletype = selectedItem.PersonType == 1 ? "Cashier" : "Technician";
                    throw new BadRequestException(roletype
                             + " with same date is already exist with Name : " + NameOfEmployee.FormalName
                             + " please add/change end date of " + NameOfEmployee.FormalName
                             + " if you want to add new "
                             + roletype);
                }
                //if selected ATM member is already exist in ATM
                relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.FromPartyId == selectedItem.PersonId
                    && x.ToPartyId == selectedItem.ATMId && x.IsActive);
                if (relationship != null)
                {
                    throw new BadRequestException("This ATM Member is already exist if you want it to another role then please terminate it first by adding end date!");
                }

                relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.Id == selectedItem.RelationshipId);
                if (relationship == null)
                {
                    var user = await context.Users.FirstOrDefaultAsync(x => x.PartyId == selectedItem.PersonId);
                    if (user == null)
                    {
                        throw new Exception("Please make login Acount of selected employee first to appoint him Regional Admin!");
                    }
                    else
                    {
                        var result = await userManager.AddToRoleAsync(user, "ATMR");
                    }

                    relationship = new PartyRelationship();
                    relationship.Id = sequenceService.GetNextPartiesSequence();
                    relationship.ToPartyRole = RoleType.ATM;
                    relationship.ToPartyId = selectedItem.ATMId;
                    relationship.FromPartyId = selectedItem.PersonId;
                    relationship.FromPartyRole = selectedItem.PersonType == 1 ? RoleType.ATMCashier : RoleType.ATMTechnician;
                    relationship.StartDate = selectedItem.StartDate.GetValueOrDefault();
                    context.PartyRelationships.Add(relationship);
                }
                relationship.ThruDate = selectedItem.EndDate;
                relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                await context.SaveChangesAsync();

                return relationship.Id;

            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetPeople(int regionId, int? subRegionId, int? stationId)
        {
            var people = await(from pr in context.Parties
                                join p in context.People on pr.Id equals p.Id
                                where //p.EmploymentType == EmploymentType.Regular
                                pr.RegionId == regionId
                                && (subRegionId == null || pr.SubregionId == subRegionId)
                                && (stationId == null || pr.StationId == stationId)
                                orderby pr.FormalName
                                select new SelectListItem()
                                {
                                    IntValue = pr.Id,
                                    Text = pr.ShortName + "-" + pr.FormalName + " " + p.NationalId
                                }).ToListAsync();
            return people;
        }
        [HttpPost]
        public async Task<int> RemoveATMMember(ATMCustodianMembersOperationViewModel selectedATMMember)
        {
            PartyRelationship relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.Id == selectedATMMember.RelationshipId && x.ToPartyId == selectedATMMember.ATMId);
            if (relationship != null)
            {
                if (relationship.StartDate.Date > MyDateTime.Now.Date)
                {
                    throw new BadRequestException("You cannot remove ATMMember in previous date!");
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
        public async Task<ATMCustodianMembersOperationViewModel> GetMemberDetail(int id)
        {
            var member = from p in context.Parties
                         join r in context.PartyRelationships on p.Id equals r.FromPartyId
                         where r.Id == id && (r.FromPartyRole == RoleType.ATMCashier || r.FromPartyRole == RoleType.ATMTechnician)
                         select new ATMCustodianMembersOperationViewModel()
                         {
                             RelationshipId = r.Id,
                             PersonName = p.FormalName,
                             StartDate = r.StartDate,
                             EndDate = r.ThruDate,
                             PersonId = p.Id,
                             ATMId = r.ToPartyId
                         };
            return await member.FirstOrDefaultAsync();
        }
    }
}
