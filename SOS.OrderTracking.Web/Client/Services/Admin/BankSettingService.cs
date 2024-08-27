using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.BankSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class BankSettingService :ServiceBase, IBankSettingService 
    {
        public BankSettingService(ApiService apiService, ILogger<BankSettingService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/BankSetting";
        public async Task<BankSettingFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<BankSettingFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<BankSettingListViewModel>> GetPageAsync(BaseIndexModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<BankSettingListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(BankSettingFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, BankSettingFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
