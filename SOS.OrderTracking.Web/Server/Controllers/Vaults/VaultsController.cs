using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
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
    public class VaultsController : ControllerBase, IVaultService
    {

        private readonly AppDbContext context;

        public ILogger<VaultsController> logger { get; set; }
        private readonly SequenceService sequenceService;
        public UserManager<ApplicationUser> userManager { get; set; }

        public VaultsController(AppDbContext appDbContext,
            SequenceService sequenceService, UserManager<ApplicationUser> userManager, ILogger<VaultsController> logger)
        {
            context = appDbContext;
            this.sequenceService = sequenceService;
            this.userManager = userManager;
            this.logger = logger;
        }
        [HttpGet]
        public async Task<IndexViewModel<VaultListViewModel>> GetPageAsync([FromQuery] VaultAdditionalValueViewModel vm)
        {
            if (vm.RegionId.GetValueOrDefault() == 0)
                throw new BadRequestException("Please Select Region to see data");

            var query = (from r in context.PartyRelationships
                         from o in context.Parties.Where(x => x.Id == r.FromPartyId).DefaultIfEmpty()
                         from l in context.Parties.Where(x => x.Id == o.SubregionId).DefaultIfEmpty()
                         where r.FromPartyRole == RoleType.Vault
                         && (o.RegionId.GetValueOrDefault() == vm.RegionId)
                         && (vm.SubRegionId == null || o.SubregionId == vm.SubRegionId)
                         && r.StartDate <= vm.ThruDate
                         && (r.ThruDate == null || r.ThruDate >= vm.StartDate)
                         select new VaultListViewModel()
                         {
                             Id = o.Id,
                             Name = o.FormalName,
                             StartDate = r.StartDate,
                             ThruDate = r.ThruDate,
                             RegionId = o.RegionId,
                             SubRegionId = o.SubregionId,
                             SubRegionName = l.FormalName,
                             OrganizationType = o.Orgnization.OrganizationType == OrganizationType.Crew ? OrganizationType.VaultOnWheels : o.Orgnization.OrganizationType,
                             HasAccount = context.Users.Any(x => x.PartyId == o.Id),
                         });



            var totalRows = query.Count();
            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();
            return new IndexViewModel<VaultListViewModel>(items, totalRows);
        }
        [HttpGet]
        public async Task<VaultViewModel> GetAsync(int id)
        {
            var vault = await (from r in context.PartyRelationships
                               from o in context.Parties.Where(x => x.Id == r.FromPartyId
                               && (x.Orgnization.OrganizationType == OrganizationType.Vault
                               //|| x.Orgnization.OrganizationType == OrganizationType.VaultOnWheels
                               )).DefaultIfEmpty()
                               where r.FromPartyRole == RoleType.Vault && (id == o.Id || id == r.FromPartyId)
                               select new VaultViewModel()
                               {
                                   Id = o.Id,
                                   Name = o.FormalName,
                                   StartDate = r.StartDate,
                                   ThruDate = r.ThruDate,
                                   RegionId = o.RegionId,
                                   SubRegionId = o.SubregionId,
                                   RelationshipId = r.Id,
                                   ActiveCrewId = context.Orgnizations.FirstOrDefault(x => x.Id == r.FromPartyId && x.OrganizationType == OrganizationType.Crew).Id,
                               }).FirstAsync();
            if (vault.ActiveCrewId.GetValueOrDefault() > 0)
                vault.VaultTypeId = 2;
            else
                vault.VaultTypeId = 1;

            return vault;
        }
        [HttpPost]
        public async Task<int> PostAsync(VaultViewModel SelectedItem)
        {
            PartyRelationship relationship = null;
            Party party = null;
            AssetAllocation assetAllocation = null;

            try
            {
                if (SelectedItem.VaultTypeId == 0)
                    throw new BadRequestException("Please select vault type!");

                //if (SelectedItem.Id.GetValueOrDefault() == 0)
                if (SelectedItem.RelationshipId == 0)
                {
                    relationship = AddRelation();
                    if (SelectedItem.VaultTypeId == 1)
                    {
                        party = AddParty(SelectedItem);
                        relationship.FromPartyId = party.Id;
                    }
                    else
                        relationship.FromPartyId = SelectedItem.ActiveCrewId.GetValueOrDefault();
                }
                else
                {
                    party = await context.Parties.FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.Id == SelectedItem.RelationshipId);
                    assetAllocation = await context.AssetAllocations.FirstOrDefaultAsync(x => x.PartyId == SelectedItem.Id);

                    if (SelectedItem.VaultTypeId == 1)
                    {
                        if (party == null)
                        {
                            if (relationship != null)
                                context.PartyRelationships.Remove(relationship);
                            await context.SaveChangesAsync();

                            party = AddParty(SelectedItem);

                            relationship = AddRelation();
                        }

                        relationship.FromPartyId = party.Id;
                    }
                    else// if (SelectedItem.VaultTypeId == 2)
                    {

                        if (party != null)
                        {
                            if (relationship != null)
                                context.PartyRelationships.Remove(relationship);
                            if (party != null)
                                context.Parties.Remove(party);

                            await context.SaveChangesAsync();
                            relationship = AddRelation();
                        }

                        relationship.FromPartyId = SelectedItem.ActiveCrewId.GetValueOrDefault();
                    }
                }
                relationship.StartDate = SelectedItem.StartDate.Value;
                relationship.ThruDate = SelectedItem.ThruDate;
                relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);

                await context.SaveChangesAsync();

                return 1;//party.Id;//Ok();
            }
            catch (Exception)
            {
                // return BadRequest(ex.Message);
                //return party.Id;
                throw;
            }
        }
        private PartyRelationship AddRelation()
        {
            var relationship = new PartyRelationship
            {
                Id = sequenceService.GetNextPartiesSequence(),
                ToPartyId = 1, // Id of SOS
                FromPartyRole = RoleType.Vault,
                ToPartyRole = RoleType.ParentOrganization
            };
            context.PartyRelationships.Add(relationship);
            return relationship;
        }
        private Party AddParty(VaultViewModel SelectedItem)
        {
            var party = new Party
            {
                Id = sequenceService.GetNextPartiesSequence(),
                FormalName = SelectedItem.Name,
                PartyType = PartyType.Organization
            };
            context.Parties.Add(party);

            // Create organization with same key
            var organization = new Organization
            {
                Id = party.Id,
                OrganizationType = OrganizationType.Vault
            };

            context.Orgnizations.Add(organization);
            party.FormalName = SelectedItem.Name;
            party.RegionId = SelectedItem.RegionId;
            party.SubregionId = SelectedItem.SubRegionId;
            //var relationship = new PartyRelationship
            //{
            //    Id = sequenceService.GetNextPartiesSequence(),
            //    ToPartyId = 1, // Id of SOS
            //    FromPartyRole = RoleType.Vault,
            //    ToPartyRole = RoleType.ParentOrganization
            //};
            //context.PartyRelationships.Add(relationship);

            //return relationship;
            return party;
        }
        public async Task<IEnumerable<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int vaultId)
        {
            // if (!regionId.HasValue)
            // return NoContent();

            var vehicles = await (from a in context.Assets
                                  where a.RegionId == regionId
                                  && (subRegionId == null || a.SubregionId == subRegionId)
                                  && (stationId == null || a.StationId == stationId)
                                  select new SelectListItem(a.Id, a.Description, a.Description)).ToListAsync();

            var vehiclesAllocated = await (from a in context.Assets
                                           join r in context.AssetAllocations on a.Id equals r.AssetId
                                           where a.AssetType == AssetType.Vehicle //&& ( a.StationId == stationId)
                                           select new SelectListItem(r.AssetId, a.Description, r.PartyId.ToString())).ToListAsync();

            vehicles.RemoveAll(c => vehiclesAllocated.ToList().Exists(n => n.IntValue == c.IntValue));
            if (vaultId > 0)
            {
                var crewOrVaultVehicle = vehiclesAllocated.FirstOrDefault(x => Convert.ToInt32(x.AdditionalValue) == vaultId);
                vehicles.Add(crewOrVaultVehicle);
            }
            return vehicles;
        }
        [HttpGet]
        public async Task<VaultUserViewModel> GetUser(int partyId)
        {
            var vaultUserViewModel = await (from u in context.Users
                                            where u.PartyId == partyId
                                            select new VaultUserViewModel()
                                            {
                                                UserName = u.UserName,
                                                IMEINumber = u.IMEINumber,
                                                IsEnabled = u.EmailConfirmed,
                                                Password = "******"
                                            }).FirstOrDefaultAsync();

            return vaultUserViewModel ?? new VaultUserViewModel();
        }
        [HttpPost]
        public async Task<bool> CreateUser(VaultUserViewModel vaultUserViewModel)
        {
            try
            {
                var applicationUser = await context.Users.FirstOrDefaultAsync(x => x.PartyId == vaultUserViewModel.VaultId);
                var isNewUser = applicationUser == null;

                applicationUser ??= new ApplicationUser
                {
                    UserName = vaultUserViewModel.UserName,
                    Email = vaultUserViewModel.UserName,
                    CreatedAt = DateTime.Now
                };

                applicationUser.UserName = vaultUserViewModel.UserName;
                applicationUser.Email = vaultUserViewModel.UserName;
                applicationUser.PartyId = vaultUserViewModel.VaultId;
                applicationUser.EmailConfirmed = vaultUserViewModel.IsEnabled;
                applicationUser.IMEINumber = vaultUserViewModel.IMEINumber;
                applicationUser.LockoutEnd = null;

                if (isNewUser)
                {
                    EnsureSucess(await userManager.CreateAsync(applicationUser, vaultUserViewModel.Password));
                }
                else
                {
                    EnsureSucess(await userManager.UpdateAsync(applicationUser));

                    if (!string.IsNullOrWhiteSpace(vaultUserViewModel.Password) && !vaultUserViewModel.Password.All(x => x == '*'))
                    {
                        await userManager.RemovePasswordAsync(applicationUser);
                        EnsureSucess(await userManager.AddPasswordAsync(applicationUser, vaultUserViewModel.Password));
                    }
                }


                var roles = await userManager.GetRolesAsync(applicationUser);
                if (roles.Count() > 0)
                    await userManager.RemoveFromRolesAsync(applicationUser, roles);

                EnsureSucess(await userManager.AddToRoleAsync(applicationUser, "CIT"));

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw new BadRequestException(ex.Message);
            }
        }
        private void EnsureSucess(IdentityResult result)
        {
            if (!result.Succeeded)
                throw new BadRequestException(string.Join("<br/>", result.Errors.Select(x => x.Description)));
        }
        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetActivecrews()
        {
            var crews = (from o in context.Parties
                         join r in context.PartyRelationships on o.Id equals r.FromPartyId
                         where r.FromPartyRole == RoleType.Crew //|| r.FromPartyRole == RoleType.Vault
                                                                //    && o.IsActive
                         select new SelectListItem()
                         {
                             Value = o.Id.ToString(),
                             Text = o.FormalName
                         });

            return await crews.ToListAsync();
        }
    }
}
