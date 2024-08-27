using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class BranchService : ServiceBase , IBranchService
    {
        public BranchService(ApiService apiService, ILogger<BranchService> logger)
            :base(apiService,logger)
        {

        }

        public override string ControllerPath => "v1/DedicatedBranches";


        //public async Task<IndexViewModel<BranchVehicleListViewModel>> GetBranchVehiclePageAsync(BaseIndexModel vm)
        //{
        //    return await ApiService.GetFromJsonAsync<IndexViewModel<BranchVehicleListViewModel>>($"{ControllerPath}/GetBranchVehiclePage?{vm.ToQueryString()}");
        //}

        public async Task<IndexViewModel<BranchesListViewModel>> GetPageAsync(BranchesAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<BranchesListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }
        public async Task<BranchesFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<BranchesFormViewModel>($"{ControllerPath}/Get?id={id}");
        }
        public async Task<int> PostAsync(BranchesFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, BranchesFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
