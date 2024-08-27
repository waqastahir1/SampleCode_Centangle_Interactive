using Duende.IdentityServer.EntityFramework.Options;
using Google.Maps;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using SOS.OrderTracking.Web.Common.Data;
using SOS.OrderTracking.Web.Common.Data.Services;
using SOS.OrderTracking.Web.Common.Extenstions;
using SOS.OrderTracking.Web.Common.Services;
using SOS.OrderTracking.Web.Common.Services.Cache;
using SOS.OrderTracking.Web.Portal.GBMS;
using System;
using System.IO;
using System.Reflection;

namespace SOS.OrderTracking.Utils
{
    public class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.Development.json");
            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                 x => x.UseNetTopologySuite())
                 .EnableSensitiveDataLogging());

            services.AddDbContext<SOS_VIEWSContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SOSViews"))
                .EnableSensitiveDataLogging());

            var storeOptions = new OperationalStoreOptions();
            services.AddSingleton(storeOptions);
            services.AddTransient<PartiesCacheService>();

            services.AddIdentityServer();

            #region logging

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = configuration.GetConnectionString("RedisConnectionString");
            });

            //services.AddLogging(config =>
            //{
            //    config.AddConfiguration();
            //    config.AddConfiguration(configuration); 
            //    config.AddConsole();
            //});

            var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Information)
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
           .Enrich.FromLogContext()
           .WriteTo.Console(theme: AnsiConsoleTheme.Literate);

            loggerConfiguration.WriteTo.File(
               @"..\logs\live.logs",
               fileSizeLimitBytes: 10_000_000,
               rollOnFileSizeLimit: true,
               shared: true,
               flushToDiskInterval: TimeSpan.FromSeconds(1));

            Log.Logger = loggerConfiguration.CreateLogger();

            #endregion

            services.AddTransient<AssetsService>();
            services.AddTransient<SequenceService>();
            services.AddSingleton<GbmsSyncService>();
            services.AddSingleton<ShipmentsPushService>();
            services.AddTransient<SequenceService>();
            services.AddTransient<ShipmentsCacheService>();
            services.AddSingleton<RelationshipStatusCronService>();
            services.AddSingleton<RecurringShipmentsService>();
            services.AddSingleton<ShipmentTypeDecesionService>();
            services.AddTransient<PartiesCacheService>();
            services.AddTransient<NotificationService>();
            services.AddTransient<ConsignmentService>();
            //services.AddSingleton<MissingShipments>();
            //services.AddSingleton<CustomTasks>();
            //services.AddSingleton<DbClean>();
            //services.AddSingleton<CacheService>();
            services.AddSingleton(Log.Logger);



            // ProtectedData
            //services.AddScoped<ILookupProtectorKeyRing, CustomLookupProtectorKeyRing>();
            //services.AddScoped<ILookupProtector, CustomLookupProtector>();
            //services.AddScoped<IPersonalDataProtector, PersonalDataProtector>();


            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<Serilog.ILogger>();
            logger.Information("Initialized");

            GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyChN7sFJjgB-dcK13j9odDWizX0acH2l3Q"));

            var gbms = serviceProvider.GetService<GbmsSyncService>();
            gbms.StartSyncing();

            var recurringShipmentsService = serviceProvider.GetService<RecurringShipmentsService>();
            recurringShipmentsService.Start();

            var relationshipStatus = serviceProvider.GetService<RelationshipStatusCronService>();
            relationshipStatus.Start();

            var shipmentsAdapter = serviceProvider.GetService<ShipmentsPushService>();
            shipmentsAdapter.Start();
            shipmentsAdapter.DeliverStuckShipments();

            var shipmentTypeDecesion = serviceProvider.GetService<ShipmentTypeDecesionService>();
            shipmentTypeDecesion.Start();

            //var missingShipmentsService = serviceProvider.GetService<MissingShipments>();
            //missingShipmentsService.StartFindingMissingShipmentsV2();

            //For Custom Tasks
            //var customTaskService = serviceProvider.GetService<CustomTasks>();
            //customTaskService.StartPopulatingAllocatedBranchesFromUserJson();
            //customTaskService.StartEncryptingData();
            //var dbCleanService = serviceProvider.GetService<DbClean>();
            //dbCleanService.StartCleaningConsignmentsBySOSTeam();
            //dbCleanService.StartCleaningDeliveredNotifications();
            //var cacheService = serviceProvider.GetService<CacheService>();
            //cacheService.StartShipmentCacheUpdateService();

            Log.Information("App version {0}", Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            Console.Read();

        }
    }
}
