using Serilog;

namespace SOS.OrderTracking.Web.Portal
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

            //.Filter.ByExcluding(c => c.Properties.Any(p => p.Value.ToString().Contains("PostLocation") ||
            //   p.Value.ToString().Contains("Notifications")))

            //   .Filter
            //   .ByExcluding(logEvent =>
            //        logEvent.Exception != null)


            .WriteTo.Console(outputTemplate: "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext:l}) {Message:lj}{NewLine}{Exception}");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
                loggerConfiguration.WriteTo.File(
                configuration["Serilog:WriteTo:FilePath:path"],
                fileSizeLimitBytes: 10_000_000,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: "{NewLine}[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext}.{Method}) {Message:lj}{NewLine}{Exception}");
            }
            Log.Logger = loggerConfiguration.CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseSerilog();
#if DEBUG
                    webBuilder.UseUrls("https://localhost:5001");
#endif
                });
    }
}
