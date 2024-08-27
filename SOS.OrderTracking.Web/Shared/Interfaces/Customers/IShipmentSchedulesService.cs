using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface IShipmentSchedulesService
        : ICrudService<ShipmentScheduleFormViewModel, ShipmentScheduleListViewModel, int, BaseIndexModel>
    {
        public Task<CitDenominationViewModel> GetDenomination(int id);
        public Task<int> AssignCrew(DeliveryCrewFormViewModel selectedItem);
        public Task<IEnumerable<ShowConsignmentsViewModel>> GetConsignments(int crewId);
        public Task<int> PostConsignmentDelivery(DeliveryFormViewModel deliveryFormViewModel);
        public Task<int> FinalizeShipment(ShipmentAdministrationViewModel model);
        public Task<BranchFormViewModel> GetBranchData(int branchId);
        public Task<int> ChangeBranchData(BranchFormViewModel editBranchViewModel);
        public Task<IEnumerable<SelectListItem>> GetCrews();
        public Task<IEnumerable<SelectListItem>> GetLocations(LocationType? locationType);
        public Task<IEnumerable<CrewWithLocation>> GetCrewsWithLocationMatrix(int consignmentId);
        public Task<IEnumerable<SelectListItem>> GetCPCBranches(int id);
        public Task<IEnumerable<SelectListItem>> GetSiblingBranches(int id1, int id2);
        public Task<int> SaveSchedule(ScheduleViewModel scheduleViewModel);
        public Task<ScheduleViewModel> GetSchedule(int consignmentId);
        public Task<int> ApproveConsignmentStatus(ConsignmentApprrovalViewModel consignmentStatusViewModel);

    }
}
