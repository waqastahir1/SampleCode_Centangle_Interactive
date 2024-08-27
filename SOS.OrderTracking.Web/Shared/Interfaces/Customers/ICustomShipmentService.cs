using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.CustomShipment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface ICustomShipmentService
        : ICrudService<CustomShipmentFormViewModel, CustomShipmentListViewModel, int, CustomShipmentAdditionalValueViewModel>
    {
        public Task<IEnumerable<SelectListItem>> GetCrews(int RegionId, int? SubRegionId, int? StationId);
        public Task<IEnumerable<CustomShipmentListViewModel>> SearchCustomShipments(CustomShipmentAdditionalValueViewModel vm);
    }
}
