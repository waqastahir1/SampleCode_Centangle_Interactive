using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IVaultService
        : ICrudService<VaultViewModel, VaultListViewModel, int, VaultAdditionalValueViewModel>
    {
        public Task<IEnumerable<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int vaultId);
        public Task<VaultUserViewModel> GetUser(int partyId);
        public Task<bool> CreateUser(VaultUserViewModel vaultUserViewModel);
        public Task<IEnumerable<SelectListItem>> GetActivecrews();
    }
}
