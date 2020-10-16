// <copyright file="Startup.cs" company="Mad Eddie Designs">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AFI.API
{
    using System;
    using AFI.Domain.Repositories.PolicyHolders;
    using AFI.Persistance.Contexts;
    using AFI.Persistance.Repositories.PolicyHolders;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Serilog;

    /// <summary>
    /// Class name.
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            this.Environment = environment;

            var b2 = new ConfigurationBuilder()
               .SetBasePath(this.Environment.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{this.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();

            this.Configuration = b2.Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(this.Configuration)

                // Log out to app insights if thats your thing
                // .WriteTo.ApplicationInsights(TelemetryConverter.Events)
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            // Log out the environment
            Log.Logger.Information($"Current evironment: {this.Environment.EnvironmentName}");
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Global services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            this.ConfigureVersioning(services);
            this.ConfigureCors(services);
            this.ConfigureDataContexts(services);
            this.ConfigureSwagger(services);
            this.ConfigureApplicationInsights(services);
            this.ConfigureIoC(services);

            services.AddControllers();
        }

        /// <summary>
        /// Configures MVC Versioning for API.
        /// </summary>
        /// <param name="services">Global services for application.</param>
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

        /// <summary>
        /// Configure  CORS for API.
        /// </summary>
        /// <param name="services">Global services for application.</param>
        public void ConfigureCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: "AFI Global",
                    builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.WithMethods("GET", "POST", "PUT", "DELETE");
                        builder.AllowAnyOrigin();
                    });
            });
        }

        /// <summary>
        /// Configure SWagger documentation.
        /// </summary>
        /// <param name="services">Global services for application.</param>
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
                    },
                });

                options.EnableAnnotations();
            });
        }

        /// <summary>
        /// Configure data context (EF.CORE Context).
        /// </summary>
        /// <param name="services">Global services for application.</param>
        public void ConfigureDataContexts(IServiceCollection services)
        {
            services.AddDbContext<AFIContext>(options => options.UseSqlServer(this.Configuration["ConnectionStrings:databaseConnection"]));

            // Remove this for prod by checking environment
            Log.Logger.Information($"DB Connection made to {this.Configuration.GetConnectionString("ConnectionStrings:databaseConnection")}");
        }

        /// <summary>
        /// Setup Application insights.
        /// </summary>
        /// <param name="services">Global services for application.</param>
        public void ConfigureApplicationInsights(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
        }

        /// <summary>
        /// Configure repositories.
        /// </summary>
        /// <param name="services">Global services for application.</param>
        public void ConfigureIoC(IServiceCollection services)
        {
            services.AddScoped<IPolicyHolderRepository, PolicyHolderRepository>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Passed through by .NET Core - Application configuration elements</param>
        /// <param name="env">Web Hosting Environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var serverAddressFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            Log.Logger.Information($"Service is listening on {string.Join(", ", serverAddressFeature.Addresses)}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(this.Configuration["Swagger:Endpoint:Url"], this.Configuration["Swagger:Endpoint:Name"]);
                    options.OAuthClientId(this.Configuration["ClientId"]);
                    options.OAuthAppName(this.Configuration["Swagger:Endpoint:Name"]);
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
