using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface ICrewService
        : ICrudService<CrewFormViewModel, CrewListViewModel, int, CrewAdditionalValueViewModel>
    {
        public Task<IEnumerable<SelectListItem>> GetVehiclesAsync(int? regionId, int? subRegionId, int? stationId, int crewId);
        public Task<bool> CreateUser(CrewUserViewModel crewUserViewModel);
        public Task<CrewUserViewModel> GetUser(int partyId);
    }
}
