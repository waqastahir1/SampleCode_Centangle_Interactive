using SOS.OrderTracking.Web.Shared.Admin.BankUsersDetail;
using SOS.OrderTracking.Web.Shared.ViewModels.Users;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IBankUsersDetailService
        : ICrudService<BankUserDetailFormViewModel, BankUserDetailListViewModel, string, UserAdditionalValueViewModel>
    {

    }
}
