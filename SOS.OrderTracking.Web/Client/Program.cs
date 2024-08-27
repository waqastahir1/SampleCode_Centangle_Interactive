using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Client.Services;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Radzen;
using SOS.OrderTracking.Web.Client.Services.Admin; 
using SOS.OrderTracking.Web.Client.Services.Customers;
using SOS.OrderTracking.Web.Client.Services.Reports;
using Blazor.Extensions;
using SOS.OrderTracking.Web.Client.Services.Gaurding;
using SOS.OrderTracking.Web.Client.Pages.Admin.Users;

namespace SOS.OrderTracking.Web.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA3Mjc3QDMxMzgyZTMyMmUzMGExamtCUGVTVlM0L1dpZW5nOUNGMzFzUXJhWklVcGdnWmRSaEY3L3NSZEU9");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("SOS.OrderTracking.Web.ServerAPI",
                client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddTransient(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("SOS.OrderTracking.Web.ServerAPI"));

            builder.Services.AddApiAuthorization();

            builder.Services.AddTransient<CommonApiService>();
            builder.Services.AddTransient<ApiService>();
            builder.Services.AddTransient<OrganizationalUnitsService>();
            builder.Services.AddTransient<CrewService>();
            builder.Services.AddTransient<BranchService>();
            builder.Services.AddTransient<BranchVehicleService>();
            builder.Services.AddTransient<VaultService>();
            builder.Services.AddTransient<InternalUserService>();
            builder.Services.AddTransient<ExternalUserService>();
            builder.Services.AddTransient<EmployeesService>();
            builder.Services.AddTransient<CrewMembersService>();
            builder.Services.AddTransient<VaultMembersService>();
            builder.Services.AddTransient<CrewCheckinService>();
            builder.Services.AddTransient<CrewCheckoutService>();
            builder.Services.AddTransient<VehicleService>();
            builder.Services.AddTransient<RegionalHeadService>();
            builder.Services.AddTransient<SubRegionalHeadService>();
            builder.Services.AddTransient<CitCardsService>();
            builder.Services.AddTransient<ShipmentBreakupsService>();
            builder.Services.AddTransient<CustomerReportService>();
            builder.Services.AddTransient<ShipmentSchedulesService>();
            builder.Services.AddTransient<CitFinalizeConsignmentsService>();
            builder.Services.AddTransient<AtmCustodianMembersService>();
            builder.Services.AddTransient<ATMCustodiansService>();
            builder.Services.AddTransient<IntraPartyDistanceService>();
            builder.Services.AddTransient<BankUserService>();
            builder.Services.AddTransient<BankSettingService>();
            builder.Services.AddTransient<VaultConsignmentService>();
            builder.Services.AddTransient<ComplaintService>();
            builder.Services.AddTransient<CustomerDashboardService>();
            builder.Services.AddTransient<SOSDashboardService>();
            builder.Services.AddTransient<ManageGaurdsService>();
            builder.Services.AddTransient<GaurdService>();
            builder.Services.AddTransient<CustomShipmentService>();
            builder.Services.AddTransient<DistanceHistoryService>();
            builder.Services.AddTransient<BankUsersDetailService>();

            builder.Services.Configure<IdentityOptions>(options =>
            options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

            //builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();

            builder.Services.AddNotifications();
            //builder.Services.AddSyncfusionBlazor();
            CultureInfo culture; 
                culture = CultureInfo.CreateSpecificCulture("en-PK");
            builder.Logging.SetMinimumLevel(LogLevel.Warning);
            await builder.Build().RunAsync();
        }
    }
}
