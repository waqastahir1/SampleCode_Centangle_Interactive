using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.BankUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class BankUserService : ServiceBase, IBankUserService
    {
        public BankUserService(ApiService apiService,ILogger<BankUserService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/BankUsers";

        public async Task<BankUserFormViewModel> GetAsync(string id)
        {
            return await ApiService.GetFromJsonAsync<BankUserFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<BankUserListViewModel>> GetPageAsync(BankUsersAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<BankUserListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<BankUserFormViewModel> GetUserAsync(string id, int roleType)
        {
            return await ApiService.GetFromJsonAsync<BankUserFormViewModel>($"{ControllerPath}/GetUser?id={id}&roleType={roleType}");
        }

        public async Task<string> PostAsync(BankUserFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<string, BankUserFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
