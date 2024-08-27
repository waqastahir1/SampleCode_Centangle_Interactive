using Microsoft.Extensions.Logging;
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
    public class ExternalUserService : ServiceBase, IExternalUserService
    {
        public ExternalUserService(ApiService apiService, ILogger<ExternalUserService> logger) : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/ExternalUsers";

        public async Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            return await ApiService.PostFromJsonAsync<bool, ChangePasswordViewModel>($"{ControllerPath}/ChangePassword", changePasswordViewModel);
        }

        public async Task<ExternalUsersViewModel> GetAsync(string id)
        {
            return await ApiService.GetFromJsonAsync<ExternalUsersViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetMainCustomers()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetMainCustomers");
        }

        public async Task<IndexViewModel<ExternalUsersListViewModel>> GetPageAsync(UserAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<ExternalUsersListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<string> PostAsync(ExternalUsersViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<string, ExternalUsersViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
