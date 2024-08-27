using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class ShipmentBreakupsService : ServiceBase, ICitGridService
    {
        public ShipmentBreakupsService(ApiService apiService,ILogger<CitCardsService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/ShipmentBreakups";

        public async Task<ShipmentFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<ShipmentFormViewModel>($"{ControllerPath}/Get?id={id}");
        }
        public async Task<IEnumerable<SelectListItem>> GetCrews(int RegionId, int? SubRegionId, int? StationId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetCrews?RegionId={RegionId}&SubRegionId={SubRegionId.GetValueOrDefault()}&StationId={StationId.GetValueOrDefault()}");
        }

        public async Task<IndexViewModel<CitGridListViewModel>> GetPageAsync(CitGridAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CitGridListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(ShipmentFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, ShipmentFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
