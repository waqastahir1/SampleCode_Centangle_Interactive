using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.IntraPartyDistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class IntraPartyDistanceService : ServiceBase, IIntraPartyDistance
    {
        public IntraPartyDistanceService(ApiService apiService,ILogger<IntraPartyDistanceService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/IntraPartyDistances";

        public async Task<IntraPartyDistanceFormViewModel> GetAsync(Tuple<int, int> id)
        {
            return await ApiService.GetFromJsonAsync<IntraPartyDistanceFormViewModel>($"{ControllerPath}/Get?item1={id.Item1}&item2={id.Item2}");
        }

        public async Task<IndexViewModel<IntraPartyDistanceListViewModel>> GetPageAsync(IntraPartyDistanceAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<IntraPartyDistanceListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<Tuple<int, int>> PostAsync(IntraPartyDistanceFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<Tuple<int, int>, IntraPartyDistanceFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
