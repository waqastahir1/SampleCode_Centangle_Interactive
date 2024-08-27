using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Client.Services.Models;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Vault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Pages.Admin.Vault
{
    public partial class Vaults
    {
        [Inject]
        public OrganizationalUnitsService OrganizationalUnitsService { get; set; }
        public string vaultName { get; set; }
        public IEnumerable<SelectListItem> Vehicles { get; set; }
        public IEnumerable<SelectListItem> ActiveCrews { get; set; }
        private List<SelectListItem> VaultType = new List<SelectListItem>()
        {
        new SelectListItem() { Text = "Fixed Vault", Value = "1" },
        new SelectListItem() { Text = "Vault on Wheels", Value = "2"},
        };
        public bool showConsignments { get; set; }
        public VaultUserViewModel VaultUserViewModel { get; set; }

        private DateTime? _startDate;
        private DateTime? StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime? _thruDate;
        private DateTime? ThruDate
        {
            get { return _thruDate; }
            set
            {
                _thruDate = value;
                NotifyPropertyChanged();
            }
        }

        public int VaultId { get; set; }

        public IEnumerable<SelectListItem> Regions { get; set; }

        public IEnumerable<SelectListItem> SubRegions { get; set; }


        public override string AdditionalParams { 
            get => $"&StartDate={StartDate:dd-MMM-yyyy}&ThruDate={ThruDate:dd-MMM-yyyy}";
            set => base.AdditionalParams = value; }

        public Vaults()
        {
            StartDate = DateTime.Today;
            ThruDate = DateTime.Today;
            PubSub.Hub.Default.Subscribe<VaultMembers>(this, async p =>
            {
                if (p == null)
                {
                    VaultId = 0;
                    await InvokeAsync(() => StateHasChanged());
                }
                else
                {
                    await LoadItems(true);
                }

            });

            PubSub.Hub.Default.Subscribe<VaultConsignment>(this, async p =>
             {
                 if (p == null)
                 {
                     VaultId = 0;
                     showConsignments = false;
                     await InvokeAsync(() => StateHasChanged());
                 }
                 else
                 {
                     await LoadItems(true);
                 }
             }); 
            PropertyChanged += async (p, q) =>
            {
                try
                {
                    if (q.PropertyName == nameof(SelectedItem) && SelectedItem != null)
                    {
                        SelectedItem.PropertyChanged += async (r, s) =>
                        {
                            if (s.PropertyName == nameof(SelectedItem.RegionId))
                            {
                                Logger.LogInformation($"{SelectedItem.RegionId} region selected");
                                SelectedItem.SubRegionId = null;
                                SelectedItem.VehicleId = 0;
                                SubRegions = await OrganizationalUnitsService.GetSubRegionsAsync(SelectedItem.RegionId);
                                if (SelectedItem.Id == 0)
                                {
                                    Logger.LogInformation($"Selected item's Id  {SelectedItem.Id}");
                                    Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, null, null, 0);
                                }
                                await InvokeAsync(() => StateHasChanged());
                            }
                            else if (s.PropertyName == nameof(SelectedItem.SubRegionId))
                            {
                                SelectedItem.VehicleId = 0;
                                Logger.LogInformation($"Selected item's Id  {SelectedItem.Id}");
                                Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, SelectedItem.SubRegionId, null, 0);

                                await InvokeAsync(() => StateHasChanged());
                            }
                        };
                    }
                    if (q.PropertyName == nameof(StartDate) || q.PropertyName == nameof(ThruDate))
                    {
                        await LoadItems(true);
                        Logger.LogInformation($"startdate : {StartDate:dd-MMM-yyyy} EndDate : {ThruDate:dd-MMM-yyyy}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            };
        }

        protected async Task OnItemClick(int? id, int? regionId, int? subRegionId, int? stationId)
        {

            Error = null;
            SelectedItem = CreateSelectedItem();

            if (id > 0)
            {
                SelectedItem.Id = id.GetValueOrDefault();
                SelectedItem.RegionId = regionId;
                SelectedItem.SubRegionId = subRegionId;
            }

            if (id.ToString() != "0" && !string.IsNullOrEmpty(id.ToString()))
            {
                SelectedItem = await ApiService.GetAsync(id.GetValueOrDefault());//GetViewModel<VaultViewModel>(ApiControllerName, id);
            }
            if (id == 0)
                SelectedItem.RegionId = 0;

            Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, SelectedItem.SubRegionId, stationId, id.GetValueOrDefault());
        }
        protected override async Task OnInitializedAsync()
        {
            Regions = await OrganizationalUnitsService.GetRegionsAsync();
            Vehicles = await ApiService.GetVehiclesAsync(0, 0, 0, 0);
            ActiveCrews = await ApiService.GetActivecrews();
            await base.OnInitializedAsync();
        }
        private async Task OnUserFormClicked(int vaultId, bool hasAccount)
        {
            VaultUserViewModel = new VaultUserViewModel();

            if (hasAccount)
            {
                IsModalBusy = true;
                VaultUserViewModel = await ApiService.GetUser(vaultId);
                IsModalBusy = false;
            }

            VaultUserViewModel.VaultId = vaultId;
        }
        private async Task OnUserFormSubmit()
        {
            try
            {
                IsModalBusy = true;
                var response = await ApiService.CreateUser(VaultUserViewModel);
                IsModalBusy = false;
                if (response)
                {
                    VaultUserViewModel = null;
                    ValidationError = null;
                    await LoadItems();
                }
            }
            catch (Exception ex)
            {
                IsModalBusy = false;
                Logger.LogInformation(ex.Message);
                ValidationError = ex.Message;
            }
        }
    }
}
