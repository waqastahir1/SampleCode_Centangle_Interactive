using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;

namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Authorize]
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class VaultMembersController : ControllerBase ,IVaultMembersService
    {
        private readonly AppDbContext context;
        private readonly SequenceService sequenceService;
        private readonly EmployeeService employeeService;

        public VaultMembersController(AppDbContext appDbContext,
            SequenceService sequenceService, EmployeeService employeeService)
        {
            context = appDbContext;
            this.sequenceService = sequenceService;
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IndexViewModel<VaultMembersListViewModel>> GetPageAsync([FromQuery] VaultAdditionalValueViewModel vm)
        {

            var query = (from r in context.PartyRelationships
                         join p in context.Parties on r.FromPartyId equals p.Id
                         from e in context.People.Where(x => x.Id == p.Id).DefaultIfEmpty()
                         where r.ToPartyId == vm.vaultId
                         select new VaultMembersListViewModel()
                         {
                             VaultId = r.ToPartyId,
                             RelationshipType = r.FromPartyRole,
                             EmployeeName = p.FormalName,
                             EmployeeId = p.Id,
                             StartDate = r.StartDate,
                             EndDate = r.ThruDate,
                             Id = r.Id,
                             NationalId = e.NationalId
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

            return new IndexViewModel<VaultMembersListViewModel>(items, totalRows);
        }


        [HttpGet]
        public async Task<VaultMembersViewModel> GetAsync(int id)
        {
            var member = await (from r in context.PartyRelationships
                                join p in context.Parties on r.FromPartyId equals p.Id
                                where r.Id == id
                                select new VaultMembersViewModel()
                                {

                                    RelationshipType = r.FromPartyRole,
                                    EmployeeName = p.FormalName,
                                    EmployeeId = p.Id,
                                    StartDate = r.StartDate,
                                    EndDate = r.ThruDate,
                                    Id = r.Id,
                                    VaultId = r.ToPartyId
                                }).FirstAsync();

            return member;
        }

        [HttpPost]
        public async Task<int> PostAsync(VaultMembersViewModel SelectedItem)
        {
            try
            {
                PartyRelationship relationship = null;

                if (SelectedItem.Id == 0)
                {
                    //if selected vault member is already exist in vault
                    relationship = context.PartyRelationships.FirstOrDefault(x => x.FromPartyId == Convert.ToInt32(SelectedItem.EmployeeId) && x.ToPartyId == SelectedItem.VaultId && x.ThruDate == null);
                    if (relationship != null)
                    {
                        throw new BadRequestException("This Vault Member is already exist with different designation");
                    }
                }
                // if end date less then start date
                if (SelectedItem.EndDate != null)
                {
                    if (DateTime.Compare((DateTime)SelectedItem.EndDate, (DateTime)SelectedItem.StartDate) < 0)
                    {
                        throw new BadRequestException("End Date should be greater then start date");
                    }
                }
                relationship = context.PartyRelationships.FirstOrDefault(x => x.Id == SelectedItem.Id);
                if (relationship == null)
                {
                    //try
                    //{
                    //    await employeeService.ChangeEmployeeRole(SelectedItem.EmployeeId, SelectedItem.VaultId,
                    //        SelectedItem.StartDate, SelectedItem.EndDate, SelectedItem.RelationshipType == RoleType.VaultIncharge);

                    //}
                    //catch (Exception ex)
                    //{
                    //    throw new BadRequestException(ex.Message);
                    //}

                    relationship = new PartyRelationship();
                    relationship.Id = sequenceService.GetNextPartiesSequence();
                    relationship.ToPartyRole = RoleType.Vault;
                    relationship.ToPartyId = SelectedItem.VaultId;
                    relationship.FromPartyId = SelectedItem.EmployeeId;
                    relationship.FromPartyRole = SelectedItem.RelationshipType;
                    relationship.StartDate = SelectedItem.StartDate.GetValueOrDefault();
                    relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                    context.PartyRelationships.Add(relationship);
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
        public async Task<IEnumerable<SelectListItem>> GetPotentialVaultMembers(int regionId, int? subRegionId, int? stationId)
        {
            var parties = await (from pr in context.Parties
                                 from p in context.People.Where(x => x.Id == pr.Id)
                                 where  pr.RegionId == regionId
                                 && pr.IsActive
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
    }
}
