using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.CIT.DedicatedBranches
{
    public partial class DedicatedVehicles
    {
        [Parameter]
        public string Branch_Code { get; set; } = string.Empty;
        [Parameter]
        public string Branch_Name { get; set; } = string.Empty;
        private int _PartyId;
        [Parameter]
        public int PartyId
        {
            get => _PartyId;
            set
            {
                _PartyId = value;
                NotifyPropertyChanged();
            }
        }
        public string VehicleId { get; set; }
        private IEnumerable<SelectListItem> Vehicles { get; set; }
        private VehicleRemoveFormViewModel vehicleRemoveFormViewModel { get; set; }
        public DedicatedVehicles()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(PartyId) && PartyId > 0)
                {
                    AdditionalParams = $"&PartyId={PartyId}";
                    await LoadItems(true);
                    await InvokeAsync(() => StateHasChanged());
                }

            };
            OnFilterApplied += async () =>
            {
                Vehicles = await ApiService.GetBranchAsset(OrganizationalUnit.RegionId.GetValueOrDefault(), OrganizationalUnit.SubRegionId.GetValueOrDefault(), OrganizationalUnit.StationId.GetValueOrDefault());
                await InvokeAsync(() => StateHasChanged());
                Logger.LogInformation("insided on Filter applied :  ");
            };
            OnFormSubmitted = (id) =>
            {
                PubSub.Hub.Default.Publish(this);
            };
        }
        protected override async Task OnInitializedAsync()
        {
            Vehicles = await ApiService.GetBranchAsset(OrganizationalUnit.RegionId.GetValueOrDefault(), OrganizationalUnit.SubRegionId.GetValueOrDefault(), OrganizationalUnit.StationId.GetValueOrDefault());
            await base.OnInitializedAsync();
        }
        protected async Task ShowConformation()
        {
            SelectedItem.PartyId = PartyId;
            await OnFormSubmit();
        }
        private async Task OnRemoveItemClicked(int Id)
        {
            Error = null;
            ValidationError = null;
            vehicleRemoveFormViewModel = await ApiService.GetVehicleDetails(Id);
        }
        private async Task RemoveVehicle()
        {
            Error = null;
            ValidationError = null;
            var responce = await ApiService.RemoveVehicle(vehicleRemoveFormViewModel);
            if (responce > 0)
            {
                vehicleRemoveFormViewModel = null;
                await LoadItems(true);
            }
        }
    }
}
