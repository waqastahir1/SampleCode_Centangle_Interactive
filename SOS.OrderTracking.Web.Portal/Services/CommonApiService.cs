using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Security.Claims;

namespace SOS.OrderTracking.Web.Portal.Services
{

    public class CommonApiService
    {
        private readonly AppDbContext context;
        private readonly SequenceService sequenceService;
        private readonly PartiesService partiesService;
        private readonly ILogger<CommonApiService> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmployeeService peopleService;

        public CommonApiService(
            AppDbContext appDbContext,

           SequenceService sequenceService,
           PartiesService partiesService,
           ILogger<CommonApiService> logger,
           UserManager<ApplicationUser> userManager, EmployeeService peopleService)
        {
            this.context = appDbContext;
            this.sequenceService = sequenceService;
            this.partiesService = partiesService;
            this.logger = logger;
            this.userManager = userManager;
            this.peopleService = peopleService;
        }

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetStations(int? subRegionId)
        {
            var subRegions = await (from p in context.Parties
                                    join o in context.Orgnizations on p.Id equals o.Id
                                    join r in context.PartyRelationships on p.Id equals r.FromPartyId
                                    where o.OrganizationType == OrganizationType.Station
                                    && (subRegionId == null || r.ToPartyId == subRegionId)
                                    select new SelectListItem(p.Id, p.FormalName, string.IsNullOrEmpty(p.Abbrevation) ? p.FormalName.Substring(0, 3) : p.Abbrevation)).ToArrayAsync();

            return (subRegions);
        }


        //Getting all vehicles which are not associated to any crew or vault
        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetVehicles(int? regionId, int? subRegionId, int? stationId, int crewOrVaultId)
        {
            if (!regionId.HasValue)
                throw new InvalidOperationException("Select Region");

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
            if (crewOrVaultId > 0)
            {
                var crewOrVaultVehicle = vehiclesAllocated.FirstOrDefault(x => Convert.ToInt32(x.AdditionalValue) == crewOrVaultId);
                vehicles.Add(crewOrVaultVehicle);
            }
            return (vehicles);
        }

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetAllEmployees()
        {
            try
            {
                var RegularEmployees = await peopleService.GetEmployeesByTypeAsync(EmploymentType.Regular);
                return (RegularEmployees);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
        }
        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetGaurds()
        {
            try
            {
                var Gaurds = await peopleService.GetEmployeesByTypeAsync(EmploymentType.Gaurd);
                return (Gaurds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
        }

        //get all banks to show in top dropdown

        //public async Task<IEnumerable<SelectListItem>> SearchCustomers(string search)
        //{
        //    try
        //    {
        //        var banks = await partiesService.GetOrganizationsByTypeAsync(OrganizationType.ExternalOrganization, search);
        //        return  (banks.Select(x => new
        //        {
        //            Id = x.IntValue,
        //            x.Text
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.ToString());
        //        throw;
        //    }
        //}

        //public async Task<IActionResult> SearchMainCustomers(string search)
        //{
        //    try
        //    {
        //        var banks = await partiesService.GetOrganizationsByTypeAsync(OrganizationType.MainCustomer, search);
        //        return Ok(banks.Select(x => new
        //        {
        //            Id = x.IntValue,
        //            x.Text
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex.ToString());
        //        return BadRequest(ex.Message);
        //    }
        //}
        //[HttpGet]
        public async Task<Web.Shared.UserInfoViewModel.UserInfo> GetUserInfo(ClaimsPrincipal User)
        {
            var CurrentLoggedInUserId = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value; //getting currently logged in user
            var user = await userManager.FindByIdAsync(CurrentLoggedInUserId);
            return (new Web.Shared.UserInfoViewModel.UserInfo
            {
                Name = user.Name,
                UserName = user.UserName,
                UserId = user.Id,
                Email = user.Email,
                PartyId = user.PartyId,
                Roles = await userManager.GetRolesAsync(user),
                CrewId = await (from p in context.Parties
                                join u in context.Users on p.Id equals u.PartyId
                                join r in context.PartyRelationships on p.Id equals r.FromPartyId
                                where r.ToPartyRole == RoleType.Crew
                                   && u.UserName == User.Identity.Name
                                select r.ToPartyId).FirstOrDefaultAsync()
            });
        }
        //[HttpGet]
        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {

            RelationshipDetailViewModel relationshipDetailViewModel = await peopleService.RelationshipDetail(employeeId, startDate);
            return (relationshipDetailViewModel);
        }

        //[HttpGet] 
        public string version()
        {
            var assemblyVersion = typeof(Program).Assembly.GetName().Version.ToString();
            return assemblyVersion[0..^2];
        }

        #region Location APIS

        //[HttpGet]
        public async Task<IEnumerable<SelectListItem>> GetLocations(LocationType? locationType)
        {
            var query = (from o in context.Locations

                         select new
                         {
                             Value = o.Id,
                             Text = o.Name,
                             o.Type
                         });

            if (locationType.HasValue)
            {
                query = query.Where(x => x.Type == locationType);
            }

            return (await query.Select(x => new SelectListItem()
            {
                Text = x.Text,
                IntValue = x.Value
            }).ToListAsync());
        }

        #endregion

    }
}
