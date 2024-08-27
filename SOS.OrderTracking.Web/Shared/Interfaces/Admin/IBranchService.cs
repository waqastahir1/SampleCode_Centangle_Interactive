using SOS.OrderTracking.Web.Shared.ViewModels.Branches;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IBranchService
        : ICrudService<BranchesFormViewModel, BranchesListViewModel, int, BranchesAdditionalValueViewModel>
    {

    }
}
