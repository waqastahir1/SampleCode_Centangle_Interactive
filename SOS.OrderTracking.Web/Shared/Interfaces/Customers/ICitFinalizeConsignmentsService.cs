using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface ICitFinalizeConsignmentsService
        : ICrudService<ShipmentFormViewModel, CitFinalizeConsignmentsListViewModel, int, CitFinalizeConsignmentsAdditionalValueModel>
    {
        public Task<IEnumerable<SelectListItem>> GetCrews(int RegionId, int? SubRegionId, int? StationId);
        public Task<int> FinalizeShipment(List<CitFinalizeConsignmentsListViewModel> citFinalizeConsignmentsListViewModels);
    }
}
