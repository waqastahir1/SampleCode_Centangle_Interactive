using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class DistanceHistoryService : ServiceBase, IDistanceHistoryService
    {
        public DistanceHistoryService(ApiService apiService, ILogger<DistanceHistoryService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/DistanceHistory";

        public async Task<DistanceHistoryFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<DistanceHistoryFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<DistanceHistoryListViewModel>> GetPageAsync(DistanceHistoryAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<DistanceHistoryListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(DistanceHistoryFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, DistanceHistoryFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
