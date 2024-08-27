using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class VehicleService : ServiceBase, IVehicleService
    {
        public VehicleService(ApiService apiService, ILogger<VehicleService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/Vehicles";

        public Task<VehiclesFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IndexViewModel<VehiclesListViewModel>> GetPageAsync(VehiclesAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<VehiclesListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public Task<int> PostAsync(VehiclesFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}