using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface ICrewCheckoutService
          : ICrudService<CrewAttendanceFormViewModel, CrewAttendanceListViewModel, int, CrewAttendanceAdditionalValueViewModel>
    {
        public Task<int> MarkAttendence(List<CrewAttendanceFormViewModel> SelectedItem);
    }
}
