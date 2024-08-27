using SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface IDistanceHistoryService
        : ICrudService<DistanceHistoryFormViewModel, DistanceHistoryListViewModel, int, DistanceHistoryAdditionalValueViewModel>
    {

    }
}
