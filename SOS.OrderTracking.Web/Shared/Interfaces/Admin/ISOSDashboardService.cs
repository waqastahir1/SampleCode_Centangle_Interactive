using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Dashboard;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface ISOSDashboardService
        : ICrudService<SOSDashboardFormViewModel, SOSDashboardListViewModel, int, BaseIndexModel>
    {

    }
}
