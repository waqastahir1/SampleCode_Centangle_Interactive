using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface ICrewMembersService
        : ICrudService<CrewMemberViewModel, CrewMemberListModel, int, CrewMemberAdditionalValueViewModel>
    {
        public Task<int> RemoveCrewMember(CrewMemberOperationsViewModel SelectedCrewMember);
        public Task<IEnumerable<SelectListItem>> GetPotentialCrewMembers(int regionId, int? subRegionId, int? stationId);
        public Task<RelationshipDetailViewModel> GetRelationshipDetail(int employeeId, DateTime? startDate);
        public Task<CrewMemberOperationsViewModel> GetMemberDetail(int id);
    }
}
