using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Admin.BankUsersDetail;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using SOS.OrderTracking.Web.Shared.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class BankUsersDetailService : ServiceBase, IBankUsersDetailService
    {
        public BankUsersDetailService(ApiService apiService,ILogger<BankUsersDetailService> logger) : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/BankUsersDetail";

        public async Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            return await ApiService.PostFromJsonAsync<bool, ChangePasswordViewModel>($"{ControllerPath}/ChangePassword", changePasswordViewModel);
        }

        public async Task<BankUserDetailFormViewModel> GetAsync(string id)
        {
            return await ApiService.GetFromJsonAsync<BankUserDetailFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetMainCustomers()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetMainCustomers");
        }

        public async Task<IndexViewModel<BankUserDetailListViewModel>> GetPageAsync(UserAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<BankUserDetailListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }
         
        public async Task<string> PostAsync(BankUserDetailFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<string, BankUserDetailFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
         
    }
}
