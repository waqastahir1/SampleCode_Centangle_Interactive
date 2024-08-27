using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class SOSDashboardService : ServiceBase, ISOSDashboardService
    {
        public SOSDashboardService(ApiService apiService, ILogger<SOSDashboardService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/SOSDashboard";

        public async Task<SOSDashboardFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<SOSDashboardFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<SOSDashboardListViewModel>> GetPageAsync(BaseIndexModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<SOSDashboardListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(SOSDashboardFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, SOSDashboardFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }


    }
}
