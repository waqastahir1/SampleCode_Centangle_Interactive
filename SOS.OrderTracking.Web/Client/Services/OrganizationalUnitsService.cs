using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services
{
    public class OrganizationalUnitsService : ApiService
    {
        string controller = "OrganizationalUnits";
        public OrganizationalUnitsService(
          HttpClient http,
          ILogger<OrganizationalUnitsService> logger,
          NavigationManager navigationManager, SignOutSessionStateManager sessionStateManager) : base(http, logger, navigationManager, sessionStateManager)
        {

        }

        /// <summary>
        /// Gets All Regions
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetRegionsAsync()
        {
            return await GetFromJsonAsync<List<SelectListItem>>
                       ($"v1/OrganizationalUnits/GetRegions");
        }

        /// <summary>
        /// Get SubRegions if regionId is null then all SubRegions will return else SubRegions which lies under the regionId will return
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetSubRegionsAsync(int? regionId = null)
        {
            try
            {
                return await Http.GetFromJsonAsync<List<SelectListItem>>
                      ($"v1/OrganizationalUnits/GetSubRegions?regionId={regionId.GetValueOrDefault()}");
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

        /// <summary>
        /// Get Stations if subRegionId is null then all Stations will return else Stations which lies under the subRegionId will return
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetStationsAsync(int? regionId, int? subRegionId = null)
        {
            try
            {

                return await Http.GetFromJsonAsync<List<SelectListItem>>
                      ($"v1/{controller}/getStations?regionId={regionId.GetValueOrDefault()}&subregionId={subRegionId}");

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
