using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Gaurding
{
    public class GaurdService : ServiceBase, IGaurdService
    {
        public GaurdService(ApiService apiService, ILogger<GaurdService> logger) : base(apiService, logger)
        {

        }

        public override string ControllerPath => "v1/Gaurds";

        public async Task<GaurdFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<GaurdFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetGaurds()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetGaurds");
        }

        public async Task<IndexViewModel<GaurdListViewModel>> GetPageAsync(GaurdAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<GaurdListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> PostAsync(GaurdFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, GaurdFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }

    }
}
