using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class ATMCustodiansService : ServiceBase, IATMCustodiansService
    {
        public ATMCustodiansService(ApiService apiService, ILogger<ATMCustodiansService> logger) : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/ATMCustodians";

        public async Task<ATMCustodiansViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<ATMCustodiansViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<ATMCustodiansListViewModel>> GetPageAsync(BaseIndexModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<ATMCustodiansListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(ATMCustodiansViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, ATMCustodiansViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
