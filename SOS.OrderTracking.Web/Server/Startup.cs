using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Models;
using SOS.OrderTracking.Web.Common.Data.Services;
using Microsoft.Extensions.Logging;
using SOS.OrderTracking.Web.Server.Hubs;
using SOS.OrderTracking.Web.Server.Services;
using SOS.OrderTracking.Web.Common.GBMS;
using SOS.OrderTracking.Web.Common.Services;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.Services;
using Serilog;
using Google.Maps;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SOS.OrderTracking.Web.Common.StaticClasses;
using SOS.OrderTracking.Web.Common.Services.Cache;
using Duende.IdentityServer.Services;

namespace SOS.OrderTracking.Web.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureDataSources(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
             x => x.UseNetTopologySuite())
             .EnableSensitiveDataLogging());

                services.AddDbContext<SOS_VIEWSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SOSViews"))
                .EnableSensitiveDataLogging());

                services.AddDistributedRedisCache(option =>
                {
                    option.Configuration = Configuration.GetConnectionString("RedisConnectionString");
                });
        }

        private void ConfigureIdentity(IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;


            })
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<ApplicationClaimsPrincipalFactory>();

            services.AddIdentityServer()
                .AddOperationalStore(options =>
                {
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // 1 hour
                })
                .AddApiAuthorization<ApplicationUser, AppDbContext>(options =>
                {
                    options.IdentityResources["openid"].UserClaims.Add("name");
                    options.ApiResources.Single().UserClaims.Add("name");
                    options.IdentityResources["openid"].UserClaims.Add("role");
                    options.ApiResources.Single().UserClaims.Add("role");

                    options.IdentityResources["openid"].UserClaims.Add(ClaimTypes.DateOfBirth);
                    options.ApiResources.Single().UserClaims.Add(ClaimTypes.DateOfBirth);

                });

            //services.AddApiAuthorization()
            // .AddAccountClaimsPrincipalFactory<RolesClaimsPrincipalFactory>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");
            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>,
                    ConfigureJwtBearerOptions>());
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            ConfigureDataSources(services);
            
            ConfigureIdentity(services);



            //services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddControllers()
                .AddNewtonsoftJson(); //for report rdlc

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

   

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "sos";
                options.Cookie.SameSite = SameSiteMode.Strict;
            });
             
            services.AddCors(options => options.AddPolicy("Cors", builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            services.AddSignalR();

            ConfigureDependencies(services);
        }


        private void ConfigureDependencies(IServiceCollection services)
        {
            services.AddSingleton<SmtpEmailManager>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<AssetsService>();
            services.AddTransient<ConsignmentService>();
            services.AddTransient<PartiesService>();
            services.AddTransient<EmployeeService>();
            services.AddTransient<SequenceService>();
            //services.AddTransient<ConsignmentHub>();
            services.AddTransient<WebPushNotificationService>();
            services.AddTransient<NotificationService>();
            services.AddSingleton<NotificationAgent>();
            services.AddSingleton<FcmNotificationAgent>();
            services.AddTransient<AtmrService>();
            services.AddTransient<PartiesCacheService>();
            services.AddTransient<UserCacheService>();
            services.AddTransient<ShipmentsCacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            AppDbContext appDbContext, ILogger<Startup> logger, 
            NotificationAgent notificationAgent,
            FcmNotificationAgent fcmNotificationAgent)
        {
            GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyChN7sFJjgB-dcK13j9odDWizX0acH2l3Q"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandler("/V1/Errors/Index");

            Bold.Licensing.BoldLicenseProvider.RegisterLicense("1FD/EutgcHmWk8UEM5biVllmW4Sp885wuoggYKoiCd4=");
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA3Mjc3QDMxMzgyZTMyMmUzMGExamtCUGVTVlM0L1dpZW5nOUNGMzFzUXJhWklVcGdnWmRSaEY3L3NSZEU9");
            app.UseSerilogRequestLogging();
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRequestCulture();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();//MapControllers(); 
                endpoints.MapHub<ConsignmentHub>("/ConsignmentHub");
                endpoints.MapFallbackToFile("index.html");
            });

            app.UseCors("Cors");

#if !DEBUG
                notificationAgent.StartFcmSerrvice();
#endif
            try
            {
                notificationAgent.StartWebPushService();
                fcmNotificationAgent.StartFcmSerrvice();
                notificationAgent.StartLocationService();
                notificationAgent.StartEscalationNotificationService();
                //appDbContext.Database.Migrate();
            }
            catch( Exception ex)
            {
                logger.LogError(ex.ToString());
            }

        }
    }


    public class ValidateSessionTimeMiddleware
    {
        private readonly RequestDelegate _next;
        TimeSpan ts = TimeSpan.FromMinutes(30);
        public ValidateSessionTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserCacheService userCache)
        {
            //if (context.Request.Path.StartsWithSegments("/v1") && context.User.Identity.IsAuthenticated)
            //{
            //    var lastTime = await userCache.GetSessionTime(context.User.Identity.Name);
            //    var diff = DateTime.UtcNow - lastTime;
            //    if (lastTime.HasValue && diff > ts)
            //    {
            //        await userCache.SetSessionTime(context.User.Identity.Name, DateTime.UtcNow);
            //        context.Response.StatusCode = 401; //UnAuthorized
            //        await context.Response.WriteAsync("Session Timeout");
            //        return;
            //    }
            //}

            //var userSecret = await userCache.GetUserSessionSecret(context.User.Identity.Name);
            //var userSecretFromCookie = context.User.FindFirst(ClaimTypes.DateOfBirth)?.Value;

            //if (!string.IsNullOrEmpty(userSecret) && !string.IsNullOrEmpty(userSecretFromCookie) && userSecret != userSecretFromCookie)
            //{
            //    context.Response.StatusCode = 401; //UnAuthorized
            //    await context.Response.WriteAsync("User logged in from other location");
            //    return;
            //}

            //await userCache.SetSessionTime(context.User.Identity.Name, DateTime.UtcNow);
            await _next.Invoke(context);
        }
    }

    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCulture(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateSessionTimeMiddleware>();
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

    //public class ProfileService : IProfileService
    //{
    //    protected UserManager<ApplicationUser> _userManager;
    //    private readonly UserCacheService userCache;

    //    public ProfileService(UserManager<ApplicationUser> userManager, UserCacheService userCache)
    //    {
    //        _userManager = userManager;
    //        this.userCache = userCache;
    //    }

    //    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    //    {
    //        var machineSecret = DateTime.Now.ToString("dd-MM-yy HH:mm:ss.ff");

    //        await userCache.SetUserSessioinSecret(context.Subject.Identity.Name, machineSecret);

    //        //>Processing
    //        var user = await _userManager.GetUserAsync(context.Subject);

    //        var claims = new List<Claim>
    //    {

    //        new Claim(ClaimTypes.DateOfBirth, machineSecret),
    //    };

    //        context.IssuedClaims.AddRange(claims);
    //    }

    //    public async Task IsActiveAsync(IsActiveContext context)
    //    {
    //        //>Processing
    //        var user = await _userManager.GetUserAsync(context.Subject);

    //        context.IsActive = (user != null) && user.IsActive;
    //    }
    //}
}
