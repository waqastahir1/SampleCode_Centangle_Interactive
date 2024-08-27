using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Google.Maps;
using Google.Maps.DistanceMatrix;
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
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using static Google.Maps.DistanceMatrix.DistanceMatrixResponse;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace SOS.OrderTracking.Web.Server.Controllers
{


    [Authorize]
    [ApiController]
    [Route("v1/[controller]/[action]")]
    public class CrewsController : ControllerBase, ICrewService
    {

        private readonly AppDbContext context;
        private readonly PartiesService partiesService;
        private readonly ILogger<CrewsController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        public CrewsController(AppDbContext context,
            PartiesService partiesService, UserManager<ApplicationUser> userManager,
            ILogger<CrewsController> logger)
        {
            this.context = context;
            this.partiesService = partiesService;
            this.logger = logger;
            this.userManager = userManager;
        }
        #region CRUD methods
        [HttpGet]
        public async Task<IndexViewModel<CrewListViewModel>> GetPageAsync([FromQuery] CrewAdditionalValueViewModel vm)
        {
            if (vm.RegionId.GetValueOrDefault() == 0)
            throw new BadRequestException("Please Select Region to see data");

            var query = partiesService.GetCrews(vm.RegionId.GetValueOrDefault(),
                vm.SubRegionId.GetValueOrDefault(),
                vm.StationId.GetValueOrDefault(),
                vm.SortColumn);

            if (!vm.ShowAll)
            {
                query = query.Where(x => x.isActive == true);
            }

            var totalRows = query.Count();

            var items = await query.Skip((vm.CurrentIndex - 1) * vm.RowsPerPage).Take(vm.RowsPerPage).ToArrayAsync();

            return new IndexViewModel<CrewListViewModel>(items, totalRows);
        }
        [HttpGet]
        public async Task<CrewFormViewModel> GetAsync(int id)
        {
            var crew = await (from o in context.Parties
                              join r in context.PartyRelationships on o.Id equals r.FromPartyId
                              from a in context.AssetAllocations.Where(x => x.PartyId == o.Id).DefaultIfEmpty()
                              where r.FromPartyRole == RoleType.Crew && id == o.Id
                              select new CrewFormViewModel()
                              {
                                  Id = o.Id,
                                  Name = o.FormalName,
                                  VehicleId = a.AssetId,
                                  StartDate = r.StartDate,
                                  ThruDate = r.ThruDate,
                                  RegionId = o.RegionId,
                                  SubRegionId = o.SubregionId,
                                  StationId = o.StationId,
                                  RelationshipId = r.Id
                              }).FirstAsync();

            return crew;
        }
        [HttpPost]
        public async Task<int> PostAsync(CrewFormViewModel SelectedItem)
        {
            Organization organization = null;
            PartyRelationship relationship = null;
            Party party = null;
            AssetAllocation assetAllocation = null;

            try
            {
                if (SelectedItem.Id > 0)
                {
                    party = await context.Parties.FirstOrDefaultAsync(x => x.Id == SelectedItem.Id);
                    relationship = await context.PartyRelationships.FirstOrDefaultAsync(x => x.Id == SelectedItem.RelationshipId);
                    assetAllocation = await context.AssetAllocations.FirstOrDefaultAsync(x => x.AssetId == SelectedItem.VehicleId);
                }
                if (party == null)
                {
                    party = new Party
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        PartyType = PartyType.Organization,
                        IsActive = true,
                    };
                    context.Parties.Add(party);

                    // Create organization with same key
                    organization = new Organization
                    {
                        Id = party.Id,
                        OrganizationType = OrganizationType.Crew
                    };

                    context.Orgnizations.Add(organization);

                    relationship = new PartyRelationship
                    {
                        Id = context.Sequences.GetNextPartiesSequence(),
                        ToPartyId = 1, // Id of SOS
                        FromPartyId = party.Id,
                        FromPartyRole = RoleType.Crew,
                        ToPartyRole = RoleType.ParentOrganization
                    };

                    context.PartyRelationships.Add(relationship);
                }

                party.FormalName = SelectedItem.Name;
                party.RegionId = SelectedItem.RegionId;
                party.SubregionId = SelectedItem.SubRegionId;
                party.StationId = SelectedItem.StationId;
                relationship.StartDate = SelectedItem.StartDate.Value;
                relationship.ThruDate = SelectedItem.ThruDate;
                relationship.IsActive = relationship.StartDate <= MyDateTime.Today && (relationship.ThruDate == null || relationship.ThruDate >= MyDateTime.Today);
                await context.SaveChangesAsync();

                if (SelectedItem.VehicleId > 0)
                {
                    assetAllocation = await context.AssetAllocations.FirstOrDefaultAsync(x => x.AssetId == SelectedItem.VehicleId);
                    if (assetAllocation == null)
                    {
                        var asset = context.AssetAllocations.Where(x => x.PartyId == SelectedItem.Id);
                        if (asset != null)
                            context.AssetAllocations.RemoveRange(asset);

                        assetAllocation = new AssetAllocation
                        {
                            Id = context.Sequences.GetNextCommonSequence()
                        };
                        context.AssetAllocations.Add(assetAllocation);
                    }

                    var userId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                    assetAllocation.AssetId = SelectedItem.VehicleId;
                    assetAllocation.PartyId = party.Id;
                    assetAllocation.AllocatedFrom = (DateTime)SelectedItem.StartDate;
                    assetAllocation.AllocatedThru = SelectedItem.ThruDate;
                    assetAllocation.AllocatedBy = user.UserName;    //current logged in user name
                    assetAllocation.AllocatedAt = MyDateTime.Now;
                    await context.SaveChangesAsync();
                }
                return party.Id;
            }
            catch (Exception ex)
            {
                //  return BadRequest(ex.Message + ex.InnerException?.Message);
                return party.Id; //its temporary when return error problem gets solved it will be removed
            }
        }
         
        #endregion

        #region Extra methods 
         
        [HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int crewId)
        {
            // if (!regionId.HasValue)
            // return NoContent();

            var vehicles = await (from a in context.Assets
                                  where a.RegionId == regionId
                                  && (a.SubregionId == subRegionId)
                                  && (a.StationId == stationId)
                                  select new SelectListItem(a.Id, a.Description, a.Description)).ToListAsync();

            var vehiclesAllocated = await (from a in context.Assets
                                           join r in context.AssetAllocations on a.Id equals r.AssetId
                                           join p in context.Parties on r.PartyId equals p.Id
                                          join pr in context.PartyRelationships on p.Id equals pr.FromPartyId
                                           where a.AssetType == AssetType.Vehicle
                                           && pr.IsActive && pr.FromPartyRole == RoleType.Crew
                                           select new SelectListItem(r.AssetId, a.Description, r.PartyId.ToString())).ToListAsync();

            vehicles.RemoveAll(c => vehiclesAllocated.ToList().Exists(n => n.IntValue == c.IntValue));
            if (crewId > 0)
            {
                var crewOrVaultVehicle = vehiclesAllocated.FirstOrDefault(x => Convert.ToInt32(x.AdditionalValue) == crewId);
                vehicles.Add(crewOrVaultVehicle);
            }
            return vehicles;
        }

        [HttpPost]
        public async Task<bool> CreateUser(CrewUserViewModel crewUserViewModel)
        {
            try
            {
                var isNewUser = false;
                var applicationUser = await context.Users.FirstOrDefaultAsync(x => x.PartyId == crewUserViewModel.CrewId);
                if (applicationUser == null)
                {
                    applicationUser = await context.Users.FirstOrDefaultAsync(x => x.UserName == crewUserViewModel.UserName);
                }

                if (applicationUser == null)
                {
                    isNewUser = true;
                    applicationUser = new ApplicationUser
                    {
                        UserName = crewUserViewModel.UserName,
                        Email = crewUserViewModel.UserName,
                        CreatedAt = DateTime.Now
                    };
                }

                applicationUser.PartyId = crewUserViewModel.CrewId;
                applicationUser.EmailConfirmed = crewUserViewModel.IsEnabled;
                applicationUser.IMEINumber = crewUserViewModel.IMEINumber;
                applicationUser.LockoutEnd = null;

                if (isNewUser)
                {
                    EnsureSucess(await userManager.CreateAsync(applicationUser, crewUserViewModel.Password));
                }
                else
                {
                    EnsureSucess(await userManager.UpdateAsync(applicationUser)); 

                    if (!string.IsNullOrWhiteSpace(crewUserViewModel.Password) && !crewUserViewModel.Password.All(x => x == '*'))
                    {
                        await userManager.RemovePasswordAsync(applicationUser);
                        EnsureSucess(await userManager.AddPasswordAsync(applicationUser, crewUserViewModel.Password));
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
        public async Task<CrewUserViewModel> GetUser(int partyId)
        {
            var crewUserViewModel = await (from u in context.Users
                               where u.PartyId == partyId
                               select new CrewUserViewModel()
                               {
                                   UserName = u.UserName,
                                   IMEINumber = u.IMEINumber,
                                   IsEnabled = u.EmailConfirmed,
                                   Password = "******"
                               }).FirstOrDefaultAsync();

            return crewUserViewModel ?? new CrewUserViewModel();
        }

        #endregion
    }
}
