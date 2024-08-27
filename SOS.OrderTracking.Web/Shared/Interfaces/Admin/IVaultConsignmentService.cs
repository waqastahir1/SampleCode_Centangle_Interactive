using SOS.OrderTracking.Web.Shared.ViewModels.Vault;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IVaultConsignmentService
          : ICrudService<VaultConsignmentViewModel, VaultConsignmentListViewModel, int, VaultConsignmentAdditionalValueModel>
    {

    }
}
