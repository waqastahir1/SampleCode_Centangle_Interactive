using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.ATM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class AtmCustodianMembersService : ServiceBase, IATMCustodianMembersService
    {
        public AtmCustodianMembersService(ApiService apiService,ILogger<AtmCustodianMembersService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/AtmCustodianMembers";

        public async Task<AtmCustodianMembersFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<AtmCustodianMembersFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<ATMCustodianMembersOperationViewModel> GetMemberDetail(int id)
        {
            return await ApiService.GetFromJsonAsync<ATMCustodianMembersOperationViewModel>($"{ControllerPath}/GetMemberDetail?id={id}");
        }

        public async Task<IndexViewModel<AtmCustodianMembersListViewModel>> GetPageAsync(AtmCustodianMembersAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<AtmCustodianMembersListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<IEnumerable<SelectListItem>> GetPeople(int regionId, int? subRegionId, int? stationId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetPeople?regionId={regionId}&subRegionId={subRegionId}&stationId={stationId}");
        }

        public async Task<int> PostAsync(AtmCustodianMembersFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, AtmCustodianMembersFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }

        public async Task<int> RemoveATMMember(ATMCustodianMembersOperationViewModel selectedATMMember)
        {
            return await ApiService.PostFromJsonAsync<int, ATMCustodianMembersOperationViewModel>($"{ControllerPath}/RemoveATMMember", selectedATMMember);
        }
    }
}
