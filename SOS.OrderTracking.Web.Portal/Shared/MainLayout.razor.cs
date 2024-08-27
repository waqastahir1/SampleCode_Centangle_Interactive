using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Portal.Services;
using SOS.OrderTracking.Web.Shared.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace SOS.OrderTracking.Web.Portal.Shared
{
    public partial class MainLayout : INotifyPropertyChanged
    {
        [Inject] IJSRuntime _jsRuntime { get; set; }


        [Inject]
        protected ILogger<MainLayout> Logger { get; set; }

        [Inject]
        public IServiceScopeFactory serviceScopeFactory { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        public OrganizationUitViewModel ViewModel { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask
        {
            get
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

        protected override async Task OnParametersSetAsync()
        {
            Helpers.UrlHelper.UpdateUrl(NavigationManager.BaseUri);
            User = (await AuthenticationStateTask).User;
            if (!(User?.Identity?.IsAuthenticated).GetValueOrDefault())
            {
                NavigationManager.NavigateTo($"identity/account/signin?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}", true);
            }
            if (User != null && ViewModel.Regions == null)
            {
                try
                {
                    var organizationalUnitsService = serviceScopeFactory
                        .CreateScope()
                        .ServiceProvider
                        .GetRequiredService<OrganizationalUnitsService>();

                    organizationalUnitsService.User = User;

                    ViewModel = await organizationalUnitsService.GetUserOrganizationsAsyn();
                    ViewModel.RegionId = ViewModel.RegionId.GetValueOrDefault();

                    ViewModel.PropertyChanged += async (p, q) =>
                    {
                        try
                        {
                            var organizationalUnitsService = serviceScopeFactory
                               .CreateScope()
                               .ServiceProvider
                               .GetRequiredService<OrganizationalUnitsService>();
                            if (q.PropertyName == nameof(ViewModel.RegionId))
                            {
                                ViewModel.SubRegions = null;
                                ViewModel.Stations = null;
                                ViewModel.SubRegions = await organizationalUnitsService.GetSubRegionsAsync(ViewModel.RegionId);
                                await InvokeAsync(() => StateHasChanged());
                                ViewModel.SubRegionId = null;
                                ViewModel.StationId = null;
                            }
                            else if (q.PropertyName == nameof(ViewModel.SubRegionId))
                            {
                                ViewModel.Stations = null;

                                ViewModel.Stations = await organizationalUnitsService
                                .GetStationsAsync(ViewModel.RegionId.GetValueOrDefault(), ViewModel.SubRegionId);
                                ViewModel.StationId = null;
                                await InvokeAsync(() => StateHasChanged());
                            }
                        }
                        catch (Exception ex)
                        {
                            Error = ex.Message;
                        }
                    };
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }

                await InvokeAsync(() => StateHasChanged());

            }
            await base.OnParametersSetAsync();
        }
        private void OnLocationChanged(object sender, LocationChangedEventArgs args)
        {
            base.InvokeAsync(async () =>
            {
                await Task.Delay(1);
            });
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            NavigationManager.LocationChanged += OnLocationChanged;

            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await _jsRuntime.InvokeVoidAsync("initializeJs");
            }
            PubSub.Hub.Default.Subscribe<ApplicationUser>(async p =>
            {

                if (p.UserName == User.Identity.Name)
                {
                    NavigationManager.NavigateTo("identity/account/logout", true);
                }
            });


        }
        async Task RequestNotificationSubscriptionAsync()
        {
            Logger.LogDebug("Requesting notification subscription");
            var subscription = await _jsRuntime.InvokeAsync<NotificationSubscription>("blazorPushNotifications.requestSubscription");

            if (subscription != null)
            {
                //try
                //{
                //    var response = await Http.Http.PutAsJsonAsync("v1/notifications/subscribe", subscription);
                //    response.EnsureSuccessStatusCode();
                //    Logger.LogDebug("Rquestd notification subscription");
                //}
                //catch (AccessTokenNotAvailableException ex)
                //{
                //    ex.Redirect();
                //}
            }
            else
            {
                Logger.LogError("Requesting notification subscription Failed");
            }
        }

        public string Error { get; set; }

        public MainLayout()
        {
            ViewModel = new OrganizationUitViewModel();

#if DEBUG
            //Task.Run(async () =>
            //{
            //    while (true)
            //    {
            //        if ((User?.Identity?.IsAuthenticated).GetValueOrDefault())
            //        {
            //            var userCacheService = serviceScopeFactory
            //               .CreateScope()
            //               .ServiceProvider
            //               .GetRequiredService<UserCacheService>();

            //            var timeStamp = await userCacheService.GetUserSessionSecret(User.Identity.Name);
            //            var claim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.DateOfBirth);
            //            if (timeStamp != claim?.Value)
            //            {
            //                UserIp = await userCacheService.GetUserIp(User.Identity.Name);
            //                UserDateTime = timeStamp;
            //                ShowLogoutDialog = true;
            //                await InvokeAsync(() => StateHasChanged());
            //                await Task.Delay(5000);
            //                NavigationManager.NavigateTo("identity/account/logout", true);
            //            }
            //        }

            //       await Task.Delay(5000);
            //    }
            //});
#endif
        }

        public string UserIp { get; set; }

        public string UserDateTime { get; set; }
        public bool ShowLogoutDialog { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        ///--- logout timer

        private System.Timers.Timer timerObj;

        protected override async Task OnInitializedAsync()
        {
            // Set the Timer delay.
            timerObj = new System.Timers.Timer(900_000); // 15 minutes
            timerObj.Elapsed += UpdateTimer;
            timerObj.AutoReset = false;
            // Identify whether the user is active or inactive using onmousemove and onkeypress in JS function.
            await _jsRuntime.InvokeVoidAsync("timeOutCall", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public void TimerInterval()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ResetColor();
            // Resetting the Timer if the user in active state.
            timerObj.Stop();
            // Call the TimeInterval to logout when the user is inactive.
            timerObj.Start();
        }

        private void UpdateTimer(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(async () =>
            {
                // Log out when the user is inactive.
                var authstate = await AuthenticationStateTask;
                if (authstate != null && authstate.User.Identity.IsAuthenticated)
                {
                    NavigationManager.NavigateTo("identity/account/logout", true);
                }
            });
        }
    }
}
