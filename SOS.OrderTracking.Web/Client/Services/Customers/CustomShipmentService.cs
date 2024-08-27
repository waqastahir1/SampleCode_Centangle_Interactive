using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.CustomShipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class CustomShipmentService : ServiceBase, ICustomShipmentService
    {
        public CustomShipmentService(ApiService apiService, ILogger<CustomShipmentService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/CustomShipments";

        public async Task<CustomShipmentFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<CustomShipmentFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetCrews(int RegionId, int? SubRegionId, int? StationId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetCrews?RegionId={RegionId}&SubRegionId={SubRegionId.GetValueOrDefault()}&StationId={StationId.GetValueOrDefault()}");
        }

        public async Task<IndexViewModel<CustomShipmentListViewModel>> GetPageAsync(CustomShipmentAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CustomShipmentListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(CustomShipmentFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, CustomShipmentFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }

        public async Task<IEnumerable<CustomShipmentListViewModel>> SearchCustomShipments(CustomShipmentAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<CustomShipmentListViewModel>>($"{ControllerPath}/SearchCustomShipments?{vm.ToQueryString()}");
        }
    }
}
