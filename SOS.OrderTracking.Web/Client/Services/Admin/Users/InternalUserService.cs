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
    public class InternalUserService : ServiceBase, IInternalUserService
    {
        public InternalUserService(ApiService apiService,ILogger<InternalUserService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/InternalUsers";

        public async Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            return await ApiService.PostFromJsonAsync<bool, ChangePasswordViewModel>($"{ControllerPath}/ChangePassword", changePasswordViewModel);
        }

        public async Task<InternalUsersViewModel> GetAsync(string id)
        {
            return await ApiService.GetFromJsonAsync<InternalUsersViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetEmployees(string userId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetEmployees?userId={userId}");
        }

        public async Task<IndexViewModel<InternalUsersListModel>> GetPageAsync(UserAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<InternalUsersListModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<List<InternalUsersViewModel>> GetRolesAsync()
        {
            return await ApiService.GetFromJsonAsync<List<InternalUsersViewModel>>($"{ControllerPath}/GetRoles");
        }

        public async Task<string> PostAsync(InternalUsersViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<string, InternalUsersViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
