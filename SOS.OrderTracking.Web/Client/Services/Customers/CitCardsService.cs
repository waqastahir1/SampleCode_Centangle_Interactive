using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.Interfaces.Customers;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Services.Customers
{
    public class CitCardsService : ServiceBase, ICitCardsService
    {
        public CitCardsService(ApiService apiService,ILogger<CitCardsService> logger) : base(apiService, logger)
        {

        }
        public override string ControllerPath => "v1/liveshipments";

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

        public async Task<ShipmentFormViewModel> GetAsync(int id)
        {
            return await ApiService.GetFromJsonAsync<ShipmentFormViewModel>($"{ControllerPath}/Get?id={id}");
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

        public async Task<IndexViewModel<ConsignmentListViewModel>> GetPageAsync(CitCardsAdditionalValueViewModel vm)
        {
            return await ApiService.GetFromJsonAsync<IndexViewModel<ConsignmentListViewModel>>($"{ControllerPath}/GetPage?{vm.ToQueryString()}");
        }

        public async Task<ConsignmentListViewModel> GetShipmentFromCache(int id)
        {
            return await ApiService.GetFromJsonAsync<ConsignmentListViewModel>($"{ControllerPath}/{nameof(GetShipmentFromCache)}?id={id}");
        }

        public async Task<IEnumerable<SelectListItem>> GetSiblingBranches(int id1, int id2)
        {
            return await ApiService.GetFromJsonAsync<IEnumerable<SelectListItem>>($"{ControllerPath}/GetSiblingBranches?id1={id1}&id2={id2}");
        }

        public async Task<int> PostAsync(ShipmentFormViewModel selectedItem)
        {
            return await ApiService.PostFromJsonAsync<int, ShipmentFormViewModel>($"{ControllerPath}/Post", selectedItem);
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

        public async Task<int> PostBulkShipments(BulkShipmentsViewModel viewModel)
        {
            return await ApiService.PostFromJsonAsync<int, BulkShipmentsViewModel>($"{ControllerPath}/PostBulkShipments", viewModel);
        }
        public async Task<int> PostDeliveryTime(DeliveryTimeViewModel deliveryTimeViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, DeliveryTimeViewModel>($"{ControllerPath}/PostDeliveryTime", deliveryTimeViewModel);
        }

        public async Task<DeliveryTimeViewModel> GetDeliveryTime(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<DeliveryTimeViewModel>($"{ControllerPath}/GetDeliveryTime?consignmentId={consignmentId}");
        }

        public async Task<int> AssignVault(DeliveryVaultViewModel viewModel)
        {
            return await ApiService.PostFromJsonAsync<int, DeliveryVaultViewModel>($"{ControllerPath}/AssignVault", viewModel);
        }

        public async Task<int> PostRatings(RatingControlViewModel ratingControl)
        {
            return await ApiService.PostFromJsonAsync<int, RatingControlViewModel>($"{ControllerPath}/PostRatings", ratingControl);
        }

        public async Task<int> PostRatingCategories(RatingCategoriesViewModel categoriesViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, RatingCategoriesViewModel>($"{ControllerPath}/PostRatingCategories", categoriesViewModel);
        }

        public async Task<int> PostComment(ShipmentCommentsViewModel viewModel)
        {
            return await ApiService.PostFromJsonAsync<int, ShipmentCommentsViewModel>($"{ControllerPath}/PostComment", viewModel);
        }

        public async Task<ShipmentCommentsViewModel> GetComments(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<ShipmentCommentsViewModel>($"{ControllerPath}/GetComments?consignmentId={consignmentId}");
        }

        public async Task<RatingCategoriesViewModel> GetRatingCategories(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<RatingCategoriesViewModel>($"{ControllerPath}/GetRatingCategories?consignmentId={consignmentId}");
        }

        public async Task<MixedCurrencyViewModel> GetMixCurrency(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<MixedCurrencyViewModel>($"{ControllerPath}/GetMixCurrency?consignmentId={consignmentId}");
        }

        public async Task<int> UpdateMixCurrency(MixedCurrencyViewModel mixedCurrencyViewModel)
        {
            return await ApiService.PostFromJsonAsync<int, MixedCurrencyViewModel>($"{ControllerPath}/UpdateMixCurrency", mixedCurrencyViewModel);
        }

        public async Task<TransitTimeViewModel> GetTransitTime(int consignmentId)
        {
            return await ApiService.GetFromJsonAsync<TransitTimeViewModel>($"{ControllerPath}/GetTransitTime?consignmentId={consignmentId}");
        }
    }
}
