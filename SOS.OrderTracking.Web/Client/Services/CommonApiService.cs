using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services
{
     
    public class CommonApiService : ApiService
    {
         
        public CommonApiService(
            HttpClient http,
            ILogger<CommonApiService> logger,
            NavigationManager navigationManager,
            SignOutSessionStateManager sessionStateManager) : base(http, logger, navigationManager, sessionStateManager)
        { 

        }

        /// <summary>
        /// Gets only those crews which have all (4) members present or expelicly marked to take consignment with 3 members
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetCrews()
        {
            return await GetFromJsonAsync<List<SelectListItem>>
                      ($"v1/crews/getActivecrews");
        }

        public async Task<List<CrewWithLocation>> GetCrewsWithLocationMatrix(int consignmentId)
        {
            var crews = await Http.GetFromJsonAsync<List<CrewWithLocation>>
                         ($"v1/crews/getcrewsWithLocationMatrix?consignmentId={consignmentId}");

            return crews;
        }

        public async Task<List<SelectListItem>> GetLocations(LocationType? locationType)
        {
           return await GetFromJsonAsync<List<SelectListItem>>
                      ($"v1/common/getLocations?locationType={locationType}");

        }

        public async Task<List<SelectListItem>> GetPotentialCrewMembers(int regionId, int? SubRegionId, int? stationId )
        {
            return await GetFromJsonAsync<List<SelectListItem>>
                    ($"v1/crews/GetPotentialCrewMembers?regionId={regionId}&subregionId={SubRegionId}&stationId={stationId}");
        }
         
        /// <summary>
        /// Get Vehicles which are not allocated to any crew/vault
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int crewOrVaultId)
        {
            try
            {
                return await Http.GetFromJsonAsync<List<SelectListItem>>
                      ($"v1/common/GetVehicles?regionId={regionId}&subregionId={subRegionId}&stationId={stationId}&crewOrVaultId={crewOrVaultId}");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex.ToString());
                logger.LogError(ex.InnerException?.ToString());

            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return new List<SelectListItem>(0);
        }
      
        //GetAllEmployees
        /// <summary>
        /// Gets All Employees
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetRegularEmployeesAsync()
        {
            try
            {
                return await Http.GetFromJsonAsync<List<SelectListItem>>
                      ($"v1/common/GetAllEmployees");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex.ToString());
                logger.LogError(ex.InnerException?.ToString());

            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            return new List<SelectListItem>(0);
        }
         
    }
}
