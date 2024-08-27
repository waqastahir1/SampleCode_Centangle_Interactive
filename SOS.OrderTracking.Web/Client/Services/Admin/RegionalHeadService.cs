using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class RegionalHeadService : ServiceBase, IRegionalHeadService
    {
        public RegionalHeadService(ApiService apiService,ILogger<RegionalHeadService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/RegionalHead";

        public async Task<RegionalHeadViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<RegionalHeadViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<RegionalHeadListViewModel>> GetPageAsync(AppointHeadsAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<RegionalHeadListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<IEnumerable<SelectListItem>> GetRegularEmployeesAsync()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetRegularEmployees");
        }

        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {
            return await ApiService.GetFromJsonAsync<RelationshipDetailViewModel>($"{ControllerPath}/GetRelationshipDetail?employeeId={employeeId}&startDate={startDate?.ToString("o")}");
        }

        public async Task<int> PostAsync(RegionalHeadViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, RegionalHeadViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
