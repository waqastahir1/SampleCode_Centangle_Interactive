using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class ShipmentSchedulesService : ServiceBase, IShipmentSchedulesService
    {
        public ShipmentSchedulesService(ApiService apiService,ILogger<ShipmentSchedulesService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/ShipmentSchedules";

        public async Task<int> AssignCrew(DeliveryCrewFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, DeliveryCrewFormViewModel>($"{ControllerPath}/{nameof(AssignCrew)}", selectedItem);
        }

        public async Task<int> ChangeBranchData(BranchFormViewModel editBranchViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, BranchFormViewModel>($"{ControllerPath}/{nameof(ChangeBranchData)}", editBranchViewModel);
        }

        public async Task<DistanceUpdateResult> UpdateShipmentDistance(ShipmentAdministrationViewModel shipmentAdministrationViewModel)
        {
            return await ApiService.PostFromJsonAsync<DistanceUpdateResult, ShipmentAdministrationViewModel>($"{ControllerPath}/{nameof(UpdateShipmentDistance)}", shipmentAdministrationViewModel);
        }

        public async Task<ShipmentScheduleFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<ShipmentScheduleFormViewModel>($"{ControllerPath}/Get?id={id}");
        }

        public async Task<BranchFormViewModel> GetBranchData(int branchId)
        {
            return await ApiService.GetFromJsonAsync<BranchFormViewModel>($"{ControllerPath}/{nameof(GetBranchData)}?branchId={branchId}");
        }

        public async Task<IEnumerable<ShowConsignmentsViewModel>> GetConsignments(int crewId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<ShowConsignmentsViewModel>>($"{ControllerPath}/GetConsignments?crewId={crewId}");
        }

        public async Task<IEnumerable<SelectListItem>> GetCPCBranches(int id)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetCPCBranches?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetCrews()
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetCrews");
        }

        public async Task<IEnumerable<CrewWithLocation>> GetCrewsWithLocationMatrix(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<CrewWithLocation>>($"{ControllerPath}/GetCrewsWithLocationMatrix?consignmentId={consignmentId}");
        }

        public async Task<CitDenominationViewModel> GetDenomination(int id)
        {
            return await ApiService.GetFromJsonAsync<CitDenominationViewModel>($"{ControllerPath}/GetDenomination?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetLocations(LocationType? locationType)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetLocations?locationType={locationType}");
        }

        public async Task<IndexViewModel<ShipmentScheduleListViewModel>> GetPageAsync(BaseIndexModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<ShipmentScheduleListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<ScheduleViewModel> GetSchedule(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<ScheduleViewModel>($"{ControllerPath}/GetSchedule?consignmentId={consignmentId}");
        }

        public async Task<IEnumerable<SelectListItem>> GetSiblingBranches(int id1, int id2)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetSiblingBranches?id1={id1}&id2={id2}");
        }

        public async Task<int> PostAsync(ShipmentScheduleFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, ShipmentScheduleFormViewModel>($"{ControllerPath}/Post", selectedItem);
        }
        public async Task<int> PostConsignmentDelivery(DeliveryFormViewModel deliveryFormViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, DeliveryFormViewModel>($"{ControllerPath}/PostConsignmentDelivery", deliveryFormViewModel);
        }

        public async Task<int> PostConsignmentStatus(ConsignmentStatusViewModel consignmentStatusViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, ConsignmentStatusViewModel>($"{ControllerPath}/PostConsignmentStatus", consignmentStatusViewModel);
        }

        public async Task<int> ApproveConsignmentStatus(ConsignmentApprrovalViewModel consignmentStatusViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, ConsignmentApprrovalViewModel>($"{ControllerPath}/ApproveConsignmentStatus", consignmentStatusViewModel);
        }
        public async Task<int> SaveSchedule(ScheduleViewModel scheduleViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, ScheduleViewModel>($"{ControllerPath}/SaveSchedule", scheduleViewModel);
        }

        public Task<int> FinalizeShipment(ShipmentAdministrationViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
