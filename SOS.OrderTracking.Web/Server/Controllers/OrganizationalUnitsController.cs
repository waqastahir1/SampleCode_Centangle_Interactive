using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Exceptions;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
namespace SOS.OrderTracking.Web.Server.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OrganizationalUnitsController : ControllerBase
    {
        private readonly AppDbContext context; 

        private readonly PartiesService partiesService; 

        public OrganizationalUnitsController(AppDbContext appDbContext, 
           PartiesService partiesService)
        {
            context = appDbContext; 
            this.partiesService = partiesService; 
        }

        [HttpGet]
        public async Task<IActionResult> GetRegions()
        { 
            IEnumerable<SelectListItem> regions = null;
            if (User.IsInRole("SOS-Admin") || User.IsInRole("BANK") || User.IsInRole("SOS-Headoffice-Billing"))
            {
                regions = await partiesService.GetAllRegionsAsync();
            }
            else if (User.IsInRole("SOS-Regional-Admin"))
            {

                regions = await partiesService.GetUserOrganizations(User.Identity.Name, OrganizationType.RegionalControlCenter);
            }

            else if (User.IsInRole("SOS-SubRegional-Admin"))
            {
                var subRegions = await partiesService.GetUserOrganizations(User.Identity.Name, OrganizationType.SubRegionalControlStation);

                regions = new SelectListItem[]{ await partiesService.GetParentRegions(subRegions.FirstOrDefault().IntValue.GetValueOrDefault(),
                    OrganizationType.RegionalControlCenter) };
            }
            return Ok(regions);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubRegions(int regionId)
        {
            if (regionId == 0)
                 return BadRequest("Select Region");

            var subregions = await partiesService.GetChildOrganizations(regionId,
                    OrganizationType.SubRegionalControlStation);

            return Ok(subregions);
        }

        [HttpGet]
        public async Task<IActionResult> GetStations(int regionId, int? subRegionId)
        {
            if (regionId == 0)
                return BadRequest("Select Region");

            if (subRegionId.GetValueOrDefault() > 0)
            {
                return Ok(await partiesService.GetChildOrganizations(subRegionId,
                     OrganizationType.Station));
            }

            var subregions = await partiesService.GetChildOrganizations(regionId,
                      OrganizationType.SubRegionalControlStation);

            return Ok(await partiesService.GetChildOrganizations(subregions.Select(x => x.IntValue).ToList(),
                   OrganizationType.Station));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserOrganizations()
        {
            var vm = new OrganizationUitViewModel();
            if (User.IsInRole(Constants.Roles.ADMIN) || User.IsInRole(Constants.Roles.HEADOFFICE_BILLING))
            {
                var regions = await partiesService.GetAllRegionsAsync();
                regions.Insert(0, new SelectListItem(0, "All Regions"));
                vm.Regions = regions;
               
                vm.SubRegions = await partiesService.GetChildOrganizations(vm.RegionId,
                   OrganizationType.SubRegionalControlStation);

                if (vm.SubRegions.Count() == 1)
                    vm.SubRegionId = vm.SubRegions.FirstOrDefault()?.IntValue;

                vm.Stations = await partiesService.GetChildOrganizations(vm.SubRegions.Select(x => x.IntValue).ToList(),
              OrganizationType.Station);
            }
            else if (User.IsInRole(Constants.Roles.REGIONAL_ADMIN))
            {

                var regions = await partiesService.GetUserOrganizations(User.Identity.Name, OrganizationType.RegionalControlCenter);
                regions.Insert(0, new SelectListItem(0, "All Regions"));
                vm.Regions = regions;

                vm.SubRegions = await partiesService.GetChildOrganizations(vm.RegionId,
                   OrganizationType.SubRegionalControlStation);

                if (vm.SubRegions.Count() == 1)
                    vm.SubRegionId = vm.SubRegions.FirstOrDefault()?.IntValue;

                vm.Stations = await partiesService.GetChildOrganizations(vm.SubRegions.Select(x => x.IntValue).ToList(),
              OrganizationType.Station);
            }

            else if (User.IsInRole(Constants.Roles.SUBREGIONAL_ADMIN))
            {

                vm.SubRegions = await partiesService.GetUserOrganizations(User.Identity.Name, OrganizationType.SubRegionalControlStation);

                vm.Regions = new SelectListItem[]{ await partiesService.GetParentRegions(vm.SubRegions.FirstOrDefault().IntValue.GetValueOrDefault(),
                    OrganizationType.RegionalControlCenter) };
                vm.RegionId = vm.Regions.FirstOrDefault()?.IntValue;
                vm.SubRegionId = vm.SubRegions.FirstOrDefault()?.IntValue;

                vm.Stations = await partiesService.GetChildOrganizations(vm.SubRegions.Select(x => x.IntValue).ToList(),
                    OrganizationType.Station);
            }
            else if(User.IsInRole(Constants.Roles.BANK_CPC) || User.IsInRole(Constants.Roles.BANK_CPC_MANAGER) 
                || User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_BRANCH_MANAGER)
                || User.IsInRole(Constants.Roles.BANK_HYBRID) || User.IsInRole(Constants.Roles.BANK))
            {
                vm.Regions = await partiesService.GetExternalUserRegion(User.Identity.Name);
                vm.RegionId = vm.Regions.FirstOrDefault()?.IntValue;

                vm.SubRegions = await partiesService.GetExternalUserSubRegion(User.Identity.Name);
                vm.SubRegionId = vm.SubRegions.FirstOrDefault()?.IntValue;

                vm.Stations = await partiesService.GetExternalUserStation(User.Identity.Name);
           
                vm.StationId = vm.Stations.FirstOrDefault()?.IntValue; 
            }
            var u = await context.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            if (u?.PartyId > 0)
            {
                var p = await context.Parties.FirstOrDefaultAsync(x => x.Id == u.PartyId);
                vm.PartyId = p?.Id;
                vm.PartyName = $"{p?.ShortName} - {p?.FormalName}";
            }

            if (vm.Regions.Count() > 1)
                vm.RegionId = 0;
            else
                vm.RegionId = vm.Regions.FirstOrDefault()?.IntValue;

            return Ok(vm);
        }
    }
}
