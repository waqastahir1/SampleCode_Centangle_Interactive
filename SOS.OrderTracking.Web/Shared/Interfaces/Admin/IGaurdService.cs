using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Gaurds;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IGaurdService
        : ICrudService<GaurdFormViewModel, GaurdListViewModel, int, GaurdAdditionalValueViewModel>
    {
        public Task<IEnumerable<SelectListItem>> GetGaurds();
    }
}
