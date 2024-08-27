using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Shared;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class CrewService : ServiceBase, ICrewService
    {
        public CrewService(ApiService apiService,
               ILogger<CrewService> logger) : base(apiService,logger)
        {

        }

        public override string ControllerPath => "v1/Crews";

        public async Task<CrewFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<CrewFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<CrewListViewModel>> GetPageAsync(CrewAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CrewListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(CrewFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, CrewFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }

        /// <summary>
        /// Get Vehicles which are not allocated to any crew/vault
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int crewId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>
              ($"{ControllerPath}/GetVehicles?regionId={regionId}&subregionId={subRegionId}&stationId={stationId}&crewId={crewId}");

        }
        public async Task<bool> CreateUser(CrewUserViewModel crewUserViewModel)
        {
            return await ApiService.PostFromJsonAsync<bool, CrewUserViewModel>($"{ControllerPath}/CreateUser", crewUserViewModel);
        }

        public async Task<CrewUserViewModel> GetUser(int partyId)
        {
            return await ApiService.GetFromJsonAsync<CrewUserViewModel>($"{ControllerPath}/GetUser?partyId={partyId}");
        }
    }
}
