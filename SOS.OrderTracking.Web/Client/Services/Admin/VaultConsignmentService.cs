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
    public class VaultConsignmentService : ServiceBase, IVaultConsignmentService
    {
        public VaultConsignmentService(ApiService apiService, ILogger<VaultConsignmentService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/VaultConsignment";

        public async Task<VaultConsignmentViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<VaultConsignmentViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<VaultConsignmentListViewModel>> GetPageAsync(VaultConsignmentAdditionalValueModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<VaultConsignmentListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(VaultConsignmentViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, VaultConsignmentViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
