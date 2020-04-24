using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace WebApplicationCSST
{
    public static class Program
    {
        public static void Main()
        {
            CreateHostBuilder(null).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel()
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    //.UseHttpSys(options =>
                    //{
                    //    options.Authentication.AllowAnonymous = false;
                    //})
                    .UseSerilog(
                    (hostingContext, loggerConfiguration) =>
                    {
                        loggerConfiguration
                            .ReadFrom.Configuration(hostingContext.Configuration);
                    }
                );
                });
    }
}
