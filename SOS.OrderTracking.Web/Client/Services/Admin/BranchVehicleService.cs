using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class BranchVehicleService : ServiceBase, IBranchVehicleService
    {
        public BranchVehicleService(ApiService apiService, ILogger<BranchVehicleService> logger)
            : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/DedicatedVehicles";

        public async Task<VehicleToBranchFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<VehicleToBranchFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetBranchAsset(int regionId, int subRegionId, int stationId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetBranchAsset?regionId={regionId}&subRegionId={subRegionId}&stationId={stationId}");
        }

        public async Task<IndexViewModel<BranchVehicleListViewModel>> GetPageAsync(BranchToVehicleInputViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<BranchVehicleListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<VehicleRemoveFormViewModel> GetVehicleDetails(int Id)
        {
            return await ApiService.GetFromJsonAsync<VehicleRemoveFormViewModel>($"{ControllerPath}/GetVehicleDetails?Id={Id}");
        }

        public async Task<int> PostAsync(VehicleToBranchFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, VehicleToBranchFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }

        public async Task<int> RemoveVehicle(VehicleRemoveFormViewModel model)
        {
            return await ApiService.PostFromJsonAsync<int, VehicleRemoveFormViewModel>($"{ControllerPath}/RemoveVehicle", model);
        }
    }
}
