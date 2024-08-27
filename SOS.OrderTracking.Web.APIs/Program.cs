using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SOS.OrderTracking.Web.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                 .MinimumLevel.Information()
                 .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                 .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("PostLocation") ||
                  p.Value.ToString().Contains("Notifications")))
                .WriteTo.Console(outputTemplate: "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext:l}) {Message:lj}{NewLine}{Exception}");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                loggerConfiguration.WriteTo.File(
                @"D:\home\LogFiles\Application\apis.log",
                fileSizeLimitBytes: 50_000_000, 
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}");
            }
            Log.Logger = loggerConfiguration.CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:1000");
                });
    }
}
