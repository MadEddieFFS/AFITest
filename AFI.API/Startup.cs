using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFI.Persistance.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;

namespace AFI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Environment = environment;

            var b2 = new ConfigurationBuilder()
               .SetBasePath(Environment.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();

            Configuration = b2.Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                // Log out to app insights if thats your thing
                // .WriteTo.ApplicationInsights(TelemetryConverter.Events)
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            // Log out the environment
            Log.Logger.Information($"Current evironment: {Environment.EnvironmentName}");
        }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            ConfigureVersioning(services);
            ConfigureCors(services);
            ConfigureSwagger(services);
            ConfigureDataContexts(services);
            ConfigureApplicationInsights(services);

            services.AddControllers();
        }

        public void ConfigureVersioning(IServiceCollection services)
        {
            services.AddApiVersioning(x =>
            {
                // Minor only for now 
                x.DefaultApiVersion = new ApiVersion(0, 1);

                // Fallback to default
                x.AssumeDefaultVersionWhenUnspecified = true;
                // Should be altered in prod unless not external
                x.ReportApiVersions = true;
            });
        }
        public void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "AFI Global",
                                  builder =>
                                  {
                                      builder.AllowAnyHeader();
                                      builder.WithMethods("GET", "POST", "PUT", "DELETE");
                                      builder.AllowAnyOrigin();
                                  });
            });

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAll"));
            //});
        }
        public void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "AFI API",
                    Description = "API for all data communications",
                    TermsOfService = new Uri("https://google.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "Paul Davies",
                        Email = string.Empty,
                        Url = new Uri("https://google.com"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://google.com"),
                    }
                });


            });
        }
        public void ConfigureDataContexts(IServiceCollection services)
        {
            services.AddDbContext<AFIContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:databaseConnection"]));
            // Remove this for prod by checking environment
            Log.Logger.Information($"DB Connection made to { Configuration.GetConnectionString("ConnectionStrings:databaseConnection")}");
        }
        public void ConfigureApplicationInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serverAddressFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            Log.Logger.Information($"Service is listening on {(String.Join(", ", serverAddressFeature.Addresses))}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(Configuration["Swagger:Endpoint:Url"], Configuration["Swagger:Endpoint:Name"]);
                    options.OAuthClientId(Configuration["ClientId"]);
                    options.OAuthAppName(Configuration["Swagger:Endpoint:Name"]);
                });
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }


            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
