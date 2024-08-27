using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IVaultMembersService
        : ICrudService<VaultMembersViewModel, VaultMembersListViewModel, int, VaultAdditionalValueViewModel>
    {
        public Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate);

        public Task<IEnumerable<SelectListItem>> GetPotentialVaultMembers(int regionId, int? subRegionId, int? stationId);
    }
}
