using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Core.PatchOperationExamples;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;

namespace H2020.IPMDecisions.APG.API.Extensions
{
    internal static class ServiceExtensions
    {
        internal static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSecretKey = config["JwtSettings:SecretKey"];
            var authorizationServerUrl = config["JwtSettings:IssuerServerUrl"];
            var audiencesServerUrl = Audiences(config["JwtSettings:ValidAudiences"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = authorizationServerUrl,
                    ValidAudiences = audiencesServerUrl,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                };
            });
        }

        internal static void ConfigureCors(this IServiceCollection services, IConfiguration config)
        {
            var allowedHosts = config["AllowedHosts"];
            if (allowedHosts == null) return;

            services.AddCors(options =>
            {
                options.AddPolicy("UserProvisionCORS", builder =>
                {
                    builder
                    .WithOrigins(allowedHosts)
                    .AllowAnyHeader(); ;
                });
            });
        }

        internal static void ConfigureContentNegotiation(this IServiceCollection services)
        {
            services.AddControllers(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true;
                })
            .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                      .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.h2020ipmdecisions.hateoas+json");
                }
            });
        }

        internal static void ConfigurePostgresContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings:MyPostgreSQLConnection"];

            services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(
                        connectionString,
                            b => b.UseNetTopologySuite()
                            .MigrationsAssembly("H2020.IPMDecisions.UPR.Data")
                        );
                });
        }

        internal static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "H2020 IPM Decisions - User Provision API",
                    Version = "v1",
                    Description = "Identity Provider for the H2020 IPM Decisions project",
                    // TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "ADAS Modelling and Informatics Team",
                        Email = "software@adas.co.uk",
                        Url = new Uri("https://www.adas.uk/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under GNU General Public License v3.0",
                        Url = new Uri("https://www.gnu.org/licenses/gpl-3.0.txt"),
                    }
                });
                c.DescribeAllParametersInCamelCase();

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                });

                c.ExampleFilters();
                c.OperationFilter<SecurityRequirementsOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            });

            services.AddSwaggerGenNewtonsoftSupport();
            services.AddSwaggerExamplesFromAssemblyOf<JsonPatchFieldRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<JsonPatchFarmRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<JsonPatchUserWidgetRequestExample>();
            services.AddSwaggerExamplesFromAssemblyOf<JsonPatchUserProfileRequestExample>();
        }

        internal static void ConfigureKestrelWebServer(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<KestrelServerOptions>(
                config.GetSection("Kestrel")
            );
        }

        internal static void ConfigureHttps(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = int.Parse(config["ASPNETCORE_HTTPS_PORT"]);
            });
        }

        internal static void ConfigureLogger(this IServiceCollection services, IConfiguration config)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
        }

        internal static void ConfigureInternalCommunicationHttpService(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IMicroservicesInternalCommunicationHttpProvider, MicroservicesInternalCommunicationHttpProvider>(client =>
            {
                client.BaseAddress = new Uri(config["MicroserviceInternalCommunication:ApiGatewayAddress"]);
                client.DefaultRequestHeaders.Add(config["MicroserviceInternalCommunication:SecurityTokenCustomHeader"], config["MicroserviceInternalCommunication:SecurityToken"]);
            });
        }

        internal static void ConfigureAuthorization(this IServiceCollection services, IConfiguration config)
        {
            var claimType = config["AccessClaims:ClaimTypeName"].ToLower();
            var accessLevels = AccessLevels(config["AccessClaims:UserAccessLevels"]);

            services.AddAuthorization(options =>
            {
                accessLevels.ToList().ForEach(level =>
                {
                    options.AddPolicy(level,
                        policy =>
                        {
                            policy.RequireClaim(claimType, level.ToLower());
                        });
                });
            });
        }

        internal static void ConfigureHangfire(this IServiceCollection services, IConfiguration config)
        {
            services.AddHangfire(configuration =>
                configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseNLogLogProvider()
                .UsePostgreSqlStorage(
                    config.GetConnectionString("MyPostgreSQLConnection"),
                    new PostgreSqlStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        QueuePollInterval = new TimeSpan(0, 1, 0),
                    }
                ));

            services.AddHangfireServer(options =>
            {
                options.Queues = new[] {
                    "onthefly_schedule",
                    "onthefly_queue",
                    "deleteoldresults_schedule",
                    "weather_queue",
                    "onmemory_queue",
                    "dsserror_schedule"
                };
            });
        }

        internal static void ConfigureCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
        }

        internal static IEnumerable<string> Audiences(string audiences)
        {
            var listOfAudiences = new List<string>();
            if (string.IsNullOrEmpty(audiences)) return listOfAudiences;
            listOfAudiences = audiences.Split(';').ToList();
            return listOfAudiences;
        }

        internal static IEnumerable<string> AccessLevels(string levels)
        {
            var listOfLevels = levels.Split(';').ToList();
            return listOfLevels;
        }
    }
}