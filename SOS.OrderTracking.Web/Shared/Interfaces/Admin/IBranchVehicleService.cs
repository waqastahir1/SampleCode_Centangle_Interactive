using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Branches;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Admin
{
    public interface IBranchVehicleService : ICrudService<VehicleToBranchFormViewModel, BranchVehicleListViewModel, int, BranchToVehicleInputViewModel>
    {
        Task<IEnumerable<SelectListItem>> GetBranchAsset(int regionId, int subRegionId, int stationId);
        Task<VehicleRemoveFormViewModel> GetVehicleDetails(int Id);
        Task<int> RemoveVehicle(VehicleRemoveFormViewModel model);
    }
}
