using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface IATMCustodiansService
          : ICrudService<ATMCustodiansViewModel, ATMCustodiansListViewModel, int, BaseIndexModel>
    {

    }
}
