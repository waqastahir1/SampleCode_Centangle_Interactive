using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Reports;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Reports
{
    public class CustomerReportService : ServiceBase, ICustomerReportService
    {
        public CustomerReportService(ApiService apiService,ILogger<CustomerReportService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/CustomerReport";

        public async Task<CustomerReportViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<CustomerReportViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<CustomerReportViewModel>> GetPageAsync(CustomerReportIndexViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CustomerReportViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(CustomerReportViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, CustomerReportViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
