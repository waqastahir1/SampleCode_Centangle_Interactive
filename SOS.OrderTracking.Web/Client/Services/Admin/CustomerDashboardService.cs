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
    public class CustomerDashboardService : ServiceBase, ICustomerDashboardService
    {
        public CustomerDashboardService(ApiService apiService, ILogger<CustomerDashboardService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/CustomerDashboard";

        public async Task<CustomerDashboardFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<CustomerDashboardFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<CustomerDashboardListViewModel>> GetPageAsync(BaseIndexModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CustomerDashboardListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(CustomerDashboardFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, CustomerDashboardFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
