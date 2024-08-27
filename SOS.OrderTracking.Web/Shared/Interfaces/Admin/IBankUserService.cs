using SOS.OrderTracking.Web.Shared.ViewModels.BankUser;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IBankUserService
        : ICrudService<BankUserFormViewModel, BankUserListViewModel, string, BankUsersAdditionalValueViewModel>
    {
        public Task<BankUserFormViewModel> GetUserAsync(string id, int roleType);
    }
}
