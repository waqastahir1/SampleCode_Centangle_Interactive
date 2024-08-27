using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.BankSetting;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IBankSettingService
        : ICrudService<BankSettingFormViewModel, BankSettingListViewModel, int, BaseIndexModel>
    {

    }
}
