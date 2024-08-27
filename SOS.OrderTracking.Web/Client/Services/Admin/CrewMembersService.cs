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
    public class CrewMembersService : ServiceBase, ICrewMembersService
    {
        public CrewMembersService(ApiService apiService,ILogger<CrewMembersService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/CrewMembers";

        public async Task<CrewMemberViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<CrewMemberViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<CrewMemberOperationsViewModel> GetMemberDetail(int id)
        {
            return await ApiService.GetFromJsonAsync<CrewMemberOperationsViewModel>($"{ControllerPath}/GetMemberDetail?id={id}");
        }

        public async Task<IndexViewModel<CrewMemberListModel>> GetPageAsync(CrewMemberAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<CrewMemberListModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<IEnumerable<SelectListItem>> GetPotentialCrewMembers(int regionId, int? subRegionId, int? stationId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetPotentialCrewMembers?regionId={regionId}&subRegionId={subRegionId}&stationId={stationId}");
        }

        public async Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate)
        {
            return await ApiService.GetFromJsonAsync<RelationshipDetailViewModel>($"{ControllerPath}/GetRelationshipDetail?employeeId={employeeId}&startDate={startDate?.ToString("o")}");
        }

        public async Task<int> PostAsync(CrewMemberViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, CrewMemberViewModel>($"{ControllerPath}/Post", selectedItem);
        }

        public async Task<int> RemoveCrewMember(CrewMemberOperationsViewModel selectedCrewMember)
        {
            return await ApiService.PostFromJsonAsync<int, CrewMemberOperationsViewModel>($"{ControllerPath}/RemoveCrewMember", selectedCrewMember);
        }
    }
}
