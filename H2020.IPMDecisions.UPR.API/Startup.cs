using System;
using AutoMapper;
using H2020.IPMDecisions.APG.API.Extensions;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.BLL.ScheduleTasks;
using H2020.IPMDecisions.UPR.Core.Profiles;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace H2020.IPMDecisions.UPR.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (!CurrentEnvironment.IsDevelopment())
            {
                services.ConfigureHttps(Configuration);
            }

            services.ConfigureKestrelWebServer(Configuration);

            services.ConfigureCors(Configuration);
            services.ConfigureContentNegotiation();
            services.ConfigureJwtAuthentication(Configuration);
            services.ConfigureAuthorization(Configuration);
            services.ConfigureInternalCommunicationHttpService(Configuration);

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            services.AddAutoMapper(typeof(MainProfile));

            services.ConfigureLogger(Configuration);
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IHangfireQueueJobs, HangfireQueueJobs>();
            services.AddScoped<IBusinessLogic, BusinessLogic>();

            services.AddScoped<UserAccessingOwnDataActionFilter>();
            services.AddScoped<AddUserIdToContextFilter>();
            services.AddScoped<AddLanguageToContextFilter>();
            services.AddScoped<FarmBelongsToUserActionFilter>();
            services.AddScoped<FieldBelongsToUserActionFilter>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(serviceProvider =>
            {
                var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.ConfigurePostgresContext(Configuration);
            services.ConfigureHangfire(Configuration);
            services.ConfigureSwagger();
            services.AddDataProtection();
            services.ConfigureCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime)
        {
            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (CurrentEnvironment.IsProduction())
                {
                    app.UseForwardedHeaders();
                    app.UseHsts();
                    app.UseHttpsRedirection();
                }
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error happened. Try again later.");
                    });
                });
            }

            app.UseCors("UserProvisionCORS");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            var apiBasePath = Configuration["MicroserviceInternalCommunication:UserProvisionMicroservice"];
            app.UseSwagger(c =>
            {
                c.RouteTemplate = apiBasePath + "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/{apiBasePath}swagger/v1/swagger.json", "H2020 IPM Decisions - User Provision Service API");
                c.RoutePrefix = $"{apiBasePath}swagger";
            });

            var dashboardOptions = new DashboardOptions();
            if (!CurrentEnvironment.IsDevelopment())
            {
                dashboardOptions.Authorization = new[] { new IsAdminFilter() }; ;
                dashboardOptions.IsReadOnlyFunc = (DashboardContext context) => false;
            }

            app.UseHangfireDashboard($"/{apiBasePath}dashboard", dashboardOptions);
            HangfireJobScheduler.HangfireScheduleJobs();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            NLog.LogManager.Shutdown();
        }
    }
}
