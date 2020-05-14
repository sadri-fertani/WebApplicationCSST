using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplicationCSST.API.Hubs;
using WebApplicationCSST.API.Provider.Role;
using WebApplicationCSST.Repo;
using WebApplicationCSST.Service;
using WebApplicationCSST.Service.Configuration;

namespace WebApplicationCSST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddResponseCompression(options =>
                {
                    options.Providers.Add<BrotliCompressionProvider>();
                    options.Providers.Add<GzipCompressionProvider>();

                    options.EnableForHttps = true;
                });

            services
                .AddControllers(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.Filters.Add(new AuthorizeFilter());
                });

            services
                .AddDbContext<ApplicationDbContext>(options => 
                {
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly("WebApplicationCSST.API"));
                });


            services
                .AddMemoryCache();

            services
                .AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString = Configuration.GetConnectionString("CacheDatabaseConnection");
                    options.SchemaName = "dbo";
                    options.TableName = "CacheTable";
                });

            services
                .AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                });

            // SignalR
            services
                .AddSignalR();

            services
                .AddScoped<IEmailService, EmailService>()
                .Configure<EmailSettings>(Configuration.GetSection("Services:SendEmailService"));

            services
                .AddAuthentication(options =>
                {                    
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                })
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetValue<string>("ServeroAuth2:Authority");
                    options.Audience = Configuration.GetValue<string>("ServeroAuth2:Audience");
                });

            services
                .AddRoleAuthorization<WebApplicationRoleProvider>()
                .AddAuthorization();

            services
                .AddAutoMapper(typeof(ApplicationProfile));

            services
                .AddScoped<IUnitOfWork, UnitOfWork>();

            services
                .AddScoped<IProductService, ProductService>();

            services
                .AddScoped<IProductDetailsService, ProductDetailsService>();

            services
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder
                        .AllowCredentials();
                    });
                });

            // Register the Swagger services
            services
                .AddSwaggerDocument(options => 
                {
                    options.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "CNESST API Swagger";
                        document.Info.Description = "A simple ASP.NET Core web API";
                        document.Info.TermsOfService = "None";
                        document.Info.Contact = new NSwag.OpenApiContact
                        {
                            Name = "Sadri FERTANI",
                            Email = "sadri.fertani@cnesst.gouv.qc.ca"
                        };
                    };
                });

            services
                .AddMvcCore(options =>
                {
                    options.CacheProfiles.Add("Default30",
                        new CacheProfile()
                        {
                            Duration = 30,
                            Location = ResponseCacheLocation.Any,
                            VaryByHeader = "User-Agent"
                        });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseStaticFiles();

            app
                .UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app
                    .UseHsts();
            }

            app
                .UseHttpsRedirection();

            app
                .UseRouting();

            app
                .UseAuthentication();

            app
                .UseAuthorization();

            app
                .UseCors("AllowAll");
            app
                .UseOpenApi()
                .UseSwaggerUi3();

            app
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapHub<MessageHub>("/MessageHub");
                });
        }
    }
}
