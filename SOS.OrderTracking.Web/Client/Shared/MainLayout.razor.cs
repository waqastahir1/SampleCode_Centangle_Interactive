using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SOS.OrderTracking.Web.Client.CIT.Shipments;
using SOS.OrderTracking.Web.Client.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using SOS.OrderTracking.Web.Shared.ViewModels.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SOS.OrderTracking.Web.Client.Shared
{
    public partial class MainLayout : INotifyPropertyChanged
    {
        [Inject] IJSRuntime _jsRuntime { get; set; }

        [Inject]
        protected ApiService Http { get; set; }

        [Inject]
        protected ILogger<MainLayout> Logger { get; set; }

        [Inject]
        public OrganizationalUnitsService OrganizationalUnitsService { get; set; }


        public OrganizationUitViewModel ViewModel { get; set; }
         
        public string HunConnectionStatusColour { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get
            {
                return _authenticationStateTask;
            }
            set
            {
                _authenticationStateTask = value;
                NotifyPropertyChanged();
            }
        }
        private Task<AuthenticationState> _authenticationStateTask;
        public System.Security.Claims.ClaimsPrincipal User { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Logger.LogWarning("OnAfterRenderAsync MainLayout");
            if (firstRender)
            {
                await _jsRuntime.InvokeVoidAsync("initializeJs");

                var assemblyVersion = typeof(Program).Assembly.GetName().Version.ToString();
                var versionString = assemblyVersion[0..^2];
                var versionFromApi = await Http.Http.GetStringAsync("v1/common/version");
                if (versionFromApi != versionString)
                {
                    Error = $"New updated version of App is available, please download V{versionFromApi}";
                }
                await RequestNotificationSubscriptionAsync();
                 
            }
           
            await base.OnAfterRenderAsync(firstRender);
        } 
        async Task RequestNotificationSubscriptionAsync()
        {
            Logger.LogDebug("Requesting notification subscription");
            var subscription = await _jsRuntime.InvokeAsync<NotificationSubscription>("blazorPushNotifications.requestSubscription");

            if (subscription != null)
            {
                try
                {
                    var response = await Http.Http.PutAsJsonAsync("v1/notifications/subscribe", subscription);
                    response.EnsureSuccessStatusCode();
                    Logger.LogDebug("Rquestd notification subscription");
                }
                catch (AccessTokenNotAvailableException ex)
                {
                    ex.Redirect();
                }
            }
            else
            {
                Logger.LogError("Requesting notification subscription Failed");
            }
        }

        public string Error { get; set; }

        public MainLayout()
        {
            PropertyChanged += async (p, q) =>
            {
                if (q.PropertyName == nameof(AuthenticationStateTask) && AuthenticationStateTask != null && ViewModel == null)
                {
                    User = (await AuthenticationStateTask).User;
                    if (User != null)
                    {
                        if (Http == null)
                            Logger.LogInformation("Http is null");


                        Logger.LogInformation("Updating User Info");

                        ViewModel = await Http.GetFromJsonAsync<OrganizationUitViewModel>("v1/OrganizationalUnits/GetUserOrganizations");
                        ViewModel.RegionId = ViewModel.RegionId.GetValueOrDefault();

                        await InvokeAsync(() => StateHasChanged());
                        ViewModel.PropertyChanged += async (p, q) =>
                        {
                            if (q.PropertyName == nameof(ViewModel.RegionId))
                            {
                                ViewModel.SubRegions = null;
                                ViewModel.Stations = null;
                                ViewModel.SubRegions = await OrganizationalUnitsService.GetSubRegionsAsync(ViewModel.RegionId);
                                await InvokeAsync(() => StateHasChanged());
                                ViewModel.SubRegionId = null;
                                ViewModel.StationId = null;
                            }
                            else if (q.PropertyName == nameof(ViewModel.SubRegionId))
                            {
                                ViewModel.Stations = null;

                                ViewModel.Stations = await OrganizationalUnitsService
                                .GetStationsAsync(ViewModel.RegionId.GetValueOrDefault(), ViewModel.SubRegionId);
                                ViewModel.StationId = null;
                                await InvokeAsync(() => StateHasChanged());
                            }
                        };
                    }
                }
                else
                {
                    Logger.LogInformation("Auth state is null");
                }
            };
            PubSub.Hub.Default.Subscribe<LiveShipments>(this, async(p) =>
            {
                if(p!= null)
                {
                    HunConnectionStatusColour = p.HubConnectionStatusColour;
                    await InvokeAsync(() => StateHasChanged());
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
