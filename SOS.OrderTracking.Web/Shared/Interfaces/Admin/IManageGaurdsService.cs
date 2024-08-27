using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IManageGaurdsService
        : ICrudService<GaurdsAllocationFormViewModel, GaurdsAllocationListViewModel, int, GaurdsAllocationAdditionalValueViewModel>
    {

    }
}
