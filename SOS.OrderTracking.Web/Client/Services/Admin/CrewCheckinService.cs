using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.Interfaces.Admin;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Admin
{
    public class CrewCheckinService : ServiceBase, ICrewCheckinService
    {
        public CrewCheckinService(ApiService apiService,ILogger<CrewCheckinService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/CrewCheckin";

        public Task<CrewAttendanceFormViewModel> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IndexViewModel<CrewAttendanceListViewModel>> GetPageAsync(CrewAttendanceAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CrewAttendanceListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<int> MarkAttendence(List<CrewAttendanceFormViewModel> SelectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, List<CrewAttendanceFormViewModel>>($"{ControllerPath}/MarkAttendence", SelectedItem);
        }

        public Task<int> PostAsync(CrewAttendanceFormViewModel selectedItem)
        {
            throw new NotImplementedException();
        }
    }
}
