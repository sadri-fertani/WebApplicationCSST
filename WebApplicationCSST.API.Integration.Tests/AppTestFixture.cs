using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplicationCSST.Repo;

namespace WebApplicationCSST.API.Integration.Tests
{
    public class AppTestFixture : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                .UseStartup<Startup>()
                .UseTestServer();

                webBuilder.ConfigureServices(services =>
                {
                    services.AddDbContext<ApplicationDbContext>(opt =>
                    {
                        opt.UseInMemoryDatabase(databaseName: "InMemoryDb");
                    });
                    
                    // Build the service provider.
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database contexts
                    using (var scope = sp.CreateScope())
                    {
                        // Get DbContext
                        var appDbCtx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // Ensure the database is created.
                        appDbCtx.Database.EnsureCreated();

                        // Seed the database with some specific test data.  
                        SeedData.PopulateTestData(appDbCtx);
                    }
                });
            })
            .ConfigureAppConfiguration((hostingContext, config) => 
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange : false);
            });
        }       

    }
}
