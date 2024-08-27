using Microsoft.EntityFrameworkCore;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Shared;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.Security.Claims;

namespace SOS.OrderTracking.Web.Portal.Services
{
    public class OrganizationalUnitsService
    {


        private readonly AppDbContext context;

        private readonly PartiesService partiesService;

        public ClaimsPrincipal User { get; set; }

        public OrganizationalUnitsService(AppDbContext appDbContext,
           PartiesService partiesService)
        {
            context = appDbContext;
            this.partiesService = partiesService;

        }


        /// <summary>
        /// Gets All Regions
        /// </summary>
        /// <returns></returns>

        public async Task<IEnumerable<SelectListItem>> GetRegionsAsync()
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
            return (regions);
        }

        /// <summary>
        /// Get SubRegions if regionId is null then all SubRegions will return else SubRegions which lies under the regionId will return
        /// </summary>
        public async Task<IEnumerable<SelectListItem>> GetSubRegionsAsync(int? regionId)
        {
            if (regionId.GetValueOrDefault() == 0)
                throw new InvalidOperationException("Select Region");

            var subregions = await partiesService.GetChildOrganizations(regionId,
                    OrganizationType.SubRegionalControlStation);

            return subregions;
        }


        /// <summary>
        /// Get Stations if subRegionId is null then all Stations will return else Stations which lies under the subRegionId will return
        /// </summary>
        public async Task<IEnumerable<SelectListItem>> GetStationsAsync(int? regionId, int? subRegionId)
        {
            if (regionId.GetValueOrDefault() == 0)
                throw new InvalidOperationException("Select Region");

            if (subRegionId.GetValueOrDefault() > 0)
            {
                return (await partiesService.GetChildOrganizations(subRegionId,
                     OrganizationType.Station));
            }

            var subregions = await partiesService.GetChildOrganizations(regionId,
                      OrganizationType.SubRegionalControlStation);

            return (await partiesService.GetChildOrganizations(subregions.Select(x => x.IntValue).ToList(),
                   OrganizationType.Station));
        }


        public async Task<OrganizationUitViewModel> GetUserOrganizationsAsyn()
        {
            var vm = new OrganizationUitViewModel();
            if (User.IsInRole(Constants.Roles.ADMIN) || User.IsInRole(Constants.Roles.HEADOFFICE_BILLING) || User.IsInRole("CPC"))
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

                vm.Regions = new SelectListItem[]{ await partiesService.GetParentRegions(vm.SubRegions.FirstOrDefault()?.IntValue.GetValueOrDefault() ?? 0,
                    OrganizationType.RegionalControlCenter) };
                vm.RegionId = vm.Regions.FirstOrDefault()?.IntValue;
                vm.SubRegionId = vm.SubRegions.FirstOrDefault()?.IntValue;

                vm.Stations = await partiesService.GetChildOrganizations(vm.SubRegions.Select(x => x.IntValue).ToList(),
                    OrganizationType.Station);
            }
            else if (User.IsInRole(Constants.Roles.BANK_CPC) || User.IsInRole(Constants.Roles.BANK_CPC_MANAGER)
                || User.IsInRole(Constants.Roles.BANK_BRANCH) || User.IsInRole(Constants.Roles.BANK_BRANCH_MANAGER) || User.IsInRole(Constants.Roles.BANK_HYBRID) || User.IsInRole(Constants.Roles.BANK))
            {
                vm.Regions = await partiesService.GetExternalUserRegion(User.Identity.Name);
                vm.RegionId = vm.Regions.FirstOrDefault()?.IntValue;

                vm.SubRegions = await partiesService.GetExternalUserSubRegion(User.Identity.Name);
                vm.SubRegionId = vm.SubRegions.FirstOrDefault()?.IntValue;

                vm.Stations = await partiesService.GetExternalUserStation(User.Identity.Name);

                vm.StationId = vm.Stations.FirstOrDefault()?.IntValue;
            }
            else
            {
                throw new Exception("Stations are not defined for this Role");
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

            return (vm);
        }
    }
}
