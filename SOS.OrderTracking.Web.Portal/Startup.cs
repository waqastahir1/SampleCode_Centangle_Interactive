using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Extenstions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Portal.GBMS;
using SOS.OrderTracking.Web.Portal.Helpers;
using SOS.OrderTracking.Web.Portal.Services;
using SOS.OrderTracking.Web.Portal.Services.Customers;
using SOS.OrderTracking.Web.Server.Services;
using System.Security.Claims;

namespace SOS.OrderTracking.Web.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
           x => x.UseNetTopologySuite())
           .EnableSensitiveDataLogging());

            services.AddDbContext<SOS_VIEWSContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("SOSViews"), x => x.UseNetTopologySuite())
               .EnableSensitiveDataLogging());

            //services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration.GetConnectionString("RedisConnectionString");
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // ProtectedData
                //options.Stores.ProtectPersonalData = true;
            })
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddRoles<IdentityRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();

            // ProtectedData
            services.AddScoped<ILookupProtectorKeyRing, CustomLookupProtectorKeyRing>();
            services.AddScoped<ILookupProtector, CustomLookupProtector>();
            services.AddScoped<IPersonalDataProtector, PersonalDataProtector>();

            services.AddSession(x =>
            {
                x.Cookie.Expiration = TimeSpan.FromMinutes(15);
                x.IdleTimeout = TimeSpan.FromMinutes(15);
            });

            //While active login of previous user, who is now assigned to new branch, previous login must log out automatically to stop user from any activity.
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsPrincipalFactory>();

            services.AddTransient<CitCardsService>();

            services.AddSingleton<SmtpEmailManager>();
            services.AddTransient<AssetsService>();
            services.AddTransient<ConsignmentService>();
            services.AddTransient<PartiesService>();
            services.AddTransient<EmployeeService>();
            services.AddTransient<SequenceService>();
            services.AddTransient<OrganizationalUnitsService>();
            services.AddTransient<WebPushNotificationService>();
            services.AddTransient<NotificationService>();
            services.AddSingleton<NotificationAgent>();
            services.AddTransient<AtmrService>();
            services.AddTransient<PartiesCacheService>();
            services.AddTransient<UserCacheService>();
            services.AddTransient<ShipmentsCacheService>();
            services.AddTransient<CommonApiService>();
            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddTransient<HttpContextAccessor>();
            services.AddSingleton<IFTPUploadService>(new FTPUploadService("ftp://195.26.247.172:80", "administrator", "S()S@123$"));

            #region Ef Services 
            services.AddTransient<VaultService>();

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, NotificationAgent notificationAgent, AppDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); endpoints.MapDefaultControllerRoute();
                //endpoints.MapBlazorHub(); 
                endpoints.MapBlazorHub(options => { options.Transports = HttpTransportType.LongPolling; });

                endpoints.MapFallbackToPage("/_Host");
            });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

#if !DEBUG
            notificationAgent.StartFcmSerrvice();
            //context.Database.Migrate();
            //notificationAgent.StartWebPushService();
            notificationAgent.StartLocationService();
            notificationAgent.StartEscalationNotificationService();
            notificationAgent.StartShipmentCacheUpdateService();
#endif
            //#if !DEBUG

            //            notificationAgent.StartPasswordExpiryNotificationCreationService();
            //            notificationAgent.StartPasswordExpiryNotificationPushService();
            //#endif

        }
    }

    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly UserCacheService userCache;

        public ApplicationClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor, UserCacheService userCache)
        : base(userManager, roleManager, optionsAccessor)
        {
            this.userCache = userCache;
        }


        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var machineSecret = DateTime.Now.ToString("dd-MM-yy HH:mm:ss.ff");
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, machineSecret));
            await userCache.SetUserSessioinSecret(user.UserName, machineSecret);
            return identity;
        }
    }

}
