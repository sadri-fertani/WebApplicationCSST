using AutoMapper;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using WebApplicationCSST.API.Provider.Role;
using WebApplicationCSST.Repo;
using WebApplicationCSST.Service;

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
                .AddDbContext<ApplicationDbContext>();

            services
                .AddMemoryCache();

            services
                .AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
                });

            services
                .AddAuthentication(IISDefaults.AuthenticationScheme);

            services
                .AddRoleAuthorization<WebApplicationRoleProvider>()
                .AddAuthorization();

            services
                .AddAutoMapper(typeof(ApplicationProfile));

            services
                .AddTransient<IUnitOfWork, UnitOfWork>();

            services
                .AddScoped<IProductService, ProductService>();

            services
                .AddCors(options =>
                {
                    // Windows authentification --> Intranet --> All are my friends
                    options.AddPolicy("AllowAll", builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
                });

            services
                .AddSwaggerGen(options =>
                {
                    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "CSST API Swagger",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "Sadri FERTANI",
                            Email = "sadri.fertani@cnesst.gouv.qc.ca"
                        }
                    });
                });

            services
                .AddOData();

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

                    foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                    {
                        outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                    }

                    foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                    {
                        inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                    }
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
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.EnableDependencyInjection();
                    endpoints.Select().Filter().OrderBy().Expand().Count().MaxTop(10);
                });

            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "CSST API Swagger");
                });
        }
    }
}
