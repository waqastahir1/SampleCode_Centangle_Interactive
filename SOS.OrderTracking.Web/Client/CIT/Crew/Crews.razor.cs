using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radzen;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Client.Services.Admin;
using SOS.OrderTracking.Web.Client.Shared;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Crew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.CIT.Crew
{
    public partial class Crews
    {

        [Inject]
        private OrganizationalUnitsService OrganizationalUnitsService { get; set; }
        private bool _showAll;

        public bool showAll
        {
            get { return _showAll; }
            set { _showAll = value;
                NotifyPropertyChanged();
            }
        }

        private IEnumerable<SelectListItem> Regions { get; set; }
        private IEnumerable<SelectListItem> SubRegions { get; set; }
        private IEnumerable<SelectListItem> Stations { get; set; }
        private IEnumerable<SelectListItem> Vehicles { get; set; }
        private CrewUserViewModel CrewUserViewModel { get; set; }
        private int CrewId { get; set; }
        private string crewName { get; set; }

        public Crews()
        {

            OnSelectedItemCreated += (i) =>
            {
                Logger.LogInformation($"SelectedItem item initialized");

                SelectedItem.PropertyChanged += async (p, q) =>
                {
                    try
                    {
                        Logger.LogInformation($"{q.PropertyName} changed");
                        if (q.PropertyName == nameof(SelectedItem.RegionId))
                        {
                            try
                            {
                                Logger.LogInformation($"{SelectedItem.RegionId} region selected");
                                SelectedItem.SubRegionId = null;
                                SelectedItem.StationId = null;
                                SelectedItem.VehicleId = 0;
                                SelectedItem.Name = UpdateCrewName();
                                Logger.LogInformation($"name of region {SelectedItem.Name}");
                                SubRegions = await OrganizationalUnitsService.GetSubRegionsAsync(SelectedItem.RegionId);
                                if (SelectedItem.Id == 0)
                                {
                                    Logger.LogInformation($"Selected item's Id  {SelectedItem.Id}");
                                    Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, null, null, SelectedItem.Id);
                                }
                                await InvokeAsync(() => StateHasChanged());
                            }
                            catch (Exception ex)
                            {
                                Logger.LogWarning(ex.ToString());
                            }

                        }
                        else if (q.PropertyName == nameof(SelectedItem.SubRegionId))
                        {
                            SelectedItem.StationId = null;
                            SelectedItem.VehicleId = 0;
                            SelectedItem.Name = UpdateCrewName();
                            Stations = await OrganizationalUnitsService.GetStationsAsync(SelectedItem.RegionId, SelectedItem.SubRegionId);
                            if (SelectedItem.Id == 0)
                            {
                                Logger.LogInformation($"Selected item's Id  {SelectedItem.Id}");
                                Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, SelectedItem.SubRegionId, null, SelectedItem.Id);
                            }
                            await InvokeAsync(() => StateHasChanged());
                        }
                        else if (q.PropertyName == nameof(SelectedItem.StationId))
                        {
                            SelectedItem.VehicleId = 0;
                            SelectedItem.Name = UpdateCrewName();
                            Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, SelectedItem.SubRegionId, SelectedItem.StationId, SelectedItem.Id);
                            await InvokeAsync(() => StateHasChanged());
                        }
                        else if (q.PropertyName == nameof(SelectedItem.VehicleId))
                        {
                            SelectedItem.Name = UpdateCrewName();
                        }
                   
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }
                };
            };
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(showAll))
                {
                    AdditionalParams = $"&ShowAll={showAll}";
                    await LoadItems(true);
                }
            };
            PubSub.Hub.Default.Subscribe<CrewMembers>(this, async p =>
            {
                if (p == null)
                {
                    CrewId = 0;
                    await InvokeAsync(() => StateHasChanged());
                }
                else
                {
                    await LoadItems(true);
                }
            });
        }
        protected override async Task OnInitializedAsync()
        {
            Regions = await OrganizationalUnitsService.GetRegionsAsync();
            SubRegions = new List<SelectListItem>();
            Stations = new List<SelectListItem>();
            Vehicles = new List<SelectListItem>();
            await InvokeAsync(() => StateHasChanged());
            await base.OnInitializedAsync();
        }
        protected async Task OnItemClick(int id, int? regionId, int? subRegionId, int? stationId)
        {
            Error = null;
            ValidationError = null;
            SelectedItem = CreateSelectedItem();
            if (id > 0)
            {
                SelectedItem.Id = id;
                SelectedItem.RegionId = regionId;
                SelectedItem.SubRegionId = subRegionId;
            }

            if (id.ToString() != "0" && !string.IsNullOrEmpty(id.ToString()))
            {
                SelectedItem = await ApiService.GetAsync(id);//GetViewModel<CrewFormViewModel>(ApiControllerName, id);
            }
            if (id == 0)
                SelectedItem.RegionId = 0;

            Vehicles = await ApiService.GetVehiclesAsync(SelectedItem.RegionId, SelectedItem.SubRegionId, stationId, id);
        }

        private async Task OnUserFormClicked(int crewId, bool hasAccount)
        {
            Error = null;
            ValidationError = null;
            CrewUserViewModel = new CrewUserViewModel();
             
            if (hasAccount)
            {
                IsModalBusy = true;
                CrewUserViewModel = await ApiService.GetUser(crewId); 
                IsModalBusy = false;
            }

            CrewUserViewModel.CrewId = crewId;
        }
        private async Task OnUserFormSubmit()
        {
            try
            {
                IsModalBusy = true;
                var response = await ApiService.CreateUser(CrewUserViewModel);
                IsModalBusy = false;
                if (response)
                {
                    CrewUserViewModel = null;
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
        private string UpdateCrewName()
        {
            return ($"{Regions.FirstOrDefault(x => SelectedItem.RegionId.HasValue && x.IntValue == SelectedItem.RegionId)?.AdditionalValue}/" +
                $"{SubRegions.FirstOrDefault(x => SelectedItem.SubRegionId.HasValue && x.IntValue == SelectedItem.SubRegionId)?.AdditionalValue}/" +
                $"{Stations.FirstOrDefault(x => SelectedItem.StationId.HasValue && x.IntValue == SelectedItem.StationId)?.AdditionalValue}/" +
                $"{Vehicles.FirstOrDefault(x => x.IntValue == SelectedItem.VehicleId)?.AdditionalValue}");
        }
    }
}
