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
    public class VaultMembersService : ServiceBase, IVaultMembersService
    {
        public VaultMembersService(ApiService apiService,ILogger<VaultMembersService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/VaultMembers";

        public async Task<VaultMembersViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<VaultMembersViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<VaultMembersListViewModel>> GetPageAsync(VaultAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<VaultMembersListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<IEnumerable<SelectListItem>> GetPotentialVaultMembers(int regionId, int? subRegionId, int? stationId)
        {
            return await ApiService.GetFromJsonAsync<List<SelectListItem>>
                      ($"{ControllerPath}/{nameof(GetPotentialVaultMembers)}?regionId={regionId}&subregionId={subRegionId}&stationId={stationId}");
        }

        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {
            return await ApiService.GetFromJsonAsync<RelationshipDetailViewModel>($"{ControllerPath}/GetRelationshipDetail?employeeId={employeeId}&startDate={startDate?.ToString("o")}");
        }

        public async Task<int> PostAsync(VaultMembersViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, VaultMembersViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
