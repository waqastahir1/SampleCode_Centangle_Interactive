using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class VaultService : ServiceBase, IVaultService
    {
        public VaultService(ApiService apiService,ILogger<VaultService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/Vaults";

        public async Task<VaultViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<VaultViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<VaultListViewModel>> GetPageAsync(VaultAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<VaultListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(VaultViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, VaultViewModel>($"{ControllerPath}/Post", selectedItem);
        }

        public async Task<IEnumerable<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int vaultId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>
              ($"{ControllerPath}/GetVehicles?regionId={regionId}&subregionId={subRegionId}&stationId={stationId}&vaultId={vaultId}");
        }
        public async Task<VaultUserViewModel> GetUser(int partyId)
        {
            return await ApiService.GetFromJsonAsync<VaultUserViewModel>($"{ControllerPath}/GetUser?partyId={partyId}");
        }
        public async Task<bool> CreateUser(VaultUserViewModel vaultUserViewModel)
        {
            return await ApiService.PostFromJsonAsync<bool, VaultUserViewModel>($"{ControllerPath}/CreateUser", vaultUserViewModel);
        }

        public async Task<IEnumerable<SelectListItem>> GetActivecrews()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetActivecrews");
        }
    }

}
