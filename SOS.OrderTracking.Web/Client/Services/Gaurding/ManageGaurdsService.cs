using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Gaurding
{
    public class ManageGaurdsService : ServiceBase, IManageGaurdsService
    {
        public ManageGaurdsService(ApiService apiService, ILogger<ManageGaurdsService> logger) : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/ManageGaurds";

        public async Task<GaurdsAllocationFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<GaurdsAllocationFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<GaurdsAllocationListViewModel>> GetPageAsync(GaurdsAllocationAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<GaurdsAllocationListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(GaurdsAllocationFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, GaurdsAllocationFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }

    }
}
