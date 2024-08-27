using SOS.OrderTracking.Web.Shared.ViewModels.Vehicles;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IVehicleService
        : ICrudService<VehiclesFormViewModel, VehiclesListViewModel, int, VehiclesAdditionalValueViewModel>
    {

    }
}
