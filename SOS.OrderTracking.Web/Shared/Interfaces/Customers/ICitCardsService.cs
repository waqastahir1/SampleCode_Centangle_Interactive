using SOS.OrderTracking.Web.Shared.CIT.Shipments;
using SOS.OrderTracking.Web.Shared.Enums;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder;
using SOS.OrderTracking.Web.Shared.ViewModels.WorkOrder.Ratings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Shared.Interfaces.Customers
{
    public interface ICitCardsService
        : ICrudService<ShipmentFormViewModel, ConsignmentListViewModel, int, CitCardsAdditionalValueViewModel>
    {
        public Task<CitDenominationViewModel> GetDenomination(int id);
        public Task<int> AssignCrew(DeliveryCrewFormViewModel selectedItem);
        public Task<IEnumerable<ShowConsignmentsViewModel>> GetConsignments(int crewId);
        public Task<int> PostConsignmentDelivery(DeliveryFormViewModel deliveryFormViewModel);
        public Task<DistanceUpdateResult> UpdateShipmentDistance(ShipmentAdministrationViewModel model);
        public Task<BranchFormViewModel> GetBranchData(int branchId);
        public Task<int> ChangeBranchData(BranchFormViewModel editBranchViewModel);
        public Task<IEnumerable<SelectListItem>> GetCrews();
        public Task<IEnumerable<SelectListItem>> GetLocations(LocationType? locationType);
        public Task<IEnumerable<CrewWithLocation>> GetCrewsWithLocationMatrix(int consignmentId, bool IsALl = false, string SearchKey = null);
        public Task<IEnumerable<SelectListItem>> GetCPCBranches(int id);
        public Task<IEnumerable<SelectListItem>> GetSiblingBranches(int id1, int id2);
        public Task<int> PostConsignmentStatus(ConsignmentStatusViewModel consignmentStatusViewModel);

        public Task<int> ApproveConsignmentStatus(ConsignmentApprrovalViewModel consignmentStatusViewModel);
        public Task<int> PostBulkShipments(BulkShipmentsViewModel viewModel);
        public Task<int> PostDeliveryTime(DeliveryTimeViewModel deliveryTimeViewModel);
        public Task<DeliveryTimeViewModel> GetDeliveryTime(int consignmentId);
        public Task<int> AssignVault(VaultNowViewModel viewModel);
        public Task<int> PostRatings(RatingControlViewModel ratingControl);
        public Task<int> PostRatingCategories(RatingCategoriesViewModel categoriesViewModel);
        public Task<ShipmentCommentsViewModel> GetComments(int consignmentId);
        public Task<int> PostComment(ShipmentCommentsViewModel viewModel);
        public Task<RatingCategoriesViewModel> GetRatingCategories(int consignmentId);
        public Task<MixedCurrencyViewModel> GetMixCurrency(int consignmentId);
        public Task<int> UpdateMixCurrency(MixedCurrencyViewModel mixedCurrencyViewModel);
        public Task<TransitTimeViewModel> GetTransitTime(int consignmentId);
        public Task<IndexViewModel<CrewMemberListModel>> GetCrewMembers(int crewId);
    }
}
