using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.UserRoles;
using SOS.OrderTracking.Web.Shared.ViewModels.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IInternalUserService
        : ICrudService<InternalUsersViewModel, InternalUsersListModel, string, UserAdditionalValueViewModel>
    {
        public Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel);
        public Task<IEnumerable<SelectListItem>> GetEmployees(string userId);
        Task<List<InternalUsersViewModel>> GetRolesAsync();
    }
}
