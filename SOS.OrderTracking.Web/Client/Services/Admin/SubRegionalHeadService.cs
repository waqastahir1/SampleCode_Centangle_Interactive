using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class SubRegionalHeadService : ServiceBase, ISubRegionalHeadService
    {
        public SubRegionalHeadService(ApiService apiService, ILogger<SubRegionalHeadService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/SubRegionalHead";

        public async Task<SubRegionalHeadViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<SubRegionalHeadViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<IndexViewModel<SubRegionalHeadListViewModel>> GetPageAsync(AppointHeadsAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<SubRegionalHeadListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<IEnumerable<SelectListItem>> GetRegularEmployeesAsync()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetRegularEmployees");
        }

        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {
            return await ApiService.GetFromJsonAsync<RelationshipDetailViewModel>($"{ControllerPath}/GetRelationshipDetail?employeeId={employeeId}&startDate={startDate?.ToString("o")}");
        }

        public async Task<int> PostAsync(SubRegionalHeadViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, SubRegionalHeadViewModel>($"{ControllerPath}/Post", selectedItem);
        }
    }
}
