using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace WebApplicationCSST
{
    public static class Program
    {
        public static void Main()
        {
            CreateHostBuilder(null)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, loggerConfiguration) =>
                {
                    // clear default logging providers
                    loggerConfiguration.ClearProviders();
                    loggerConfiguration.AddEventLog(new EventLogSettings()
                    {
                        SourceName = "CNESST_API_DEMO",
                        LogName = "CNESST_API_DEMO",
                        Filter = (x, y) => y >= LogLevel.Information
                    });
                });
            });
    }
}
