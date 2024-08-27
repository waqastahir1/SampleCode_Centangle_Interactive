using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface IATMCustodianMembersService
        : ICrudService<AtmCustodianMembersFormViewModel, AtmCustodianMembersListViewModel, int, AtmCustodianMembersAdditionalValueViewModel>
    {
        public Task<IEnumerable<SelectListItem>> GetPeople(int regionId, int? subRegionId, int? stationId);
        public Task<int> RemoveATMMember(ATMCustodianMembersOperationViewModel selectedATMMember);
        public Task<ATMCustodianMembersOperationViewModel> GetMemberDetail(int id);
    }
}
