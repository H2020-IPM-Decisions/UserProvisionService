using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace H2020.IPMDecisions.APG.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSecretKey = config["JwtSettings:SecretKey"];
            var authorizationServerUrl = config["JwtSettings:IssuerServerUrl"];
            var audiencesServerUrl = Audiences(config["JwtSettings:ValidAudiencesUrls"]);

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

        public static void ConfigureCors(this IServiceCollection services, IConfiguration config)
        {
            var allowedHosts = config["AllowedHosts"];
            services.AddCors(options =>
            {
                options.AddPolicy("UserProvisionCORS", builder =>
                {
                    builder.WithOrigins(allowedHosts);
                });
            });
        }

        public static void ConfigureContentNegotiation(this IServiceCollection services)
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

        public static void ConfigurePostgresContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["ConnectionStrings:MyPostgreSQLConnection"];
            
            services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(connectionString,
                        b => b.MigrationsAssembly("H2020.IPMDecisions.UPR.Data"));
                });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
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

                c.OperationFilter<SecurityRequirementsOperationFilter>();

            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static IEnumerable<string> Audiences(string audiences)
        {
            var listOfAudiences = audiences.Split(';').ToList();
            return listOfAudiences;
        }
    }
}