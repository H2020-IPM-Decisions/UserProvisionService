using System;
using System.Threading.Tasks;
using DoomedDatabases.Postgres;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.UnitTests
{
    public class FakeWebHostForTesting : IAsyncLifetime, IDisposable
    {
        public IHost _host;
        private readonly ITestDatabase tempDatabase;
        public FakeWebHostForTesting(bool createDb = false)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.Test.json")
               .Build();

            var connectionString = configuration["ConnectionStrings:MyPostgreSQLConnection"];
            
            if (createDb)
            {
                tempDatabase = new TestDatabaseBuilder()
                .WithConnectionString(connectionString)
                .Build();

            tempDatabase.Create();
            }
            
        }

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            if (tempDatabase != null)
                configuration["ConnectionStrings:MyPostgreSQLConnection"] = tempDatabase.ConnectionString.ToString(); 

            _host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder.UseStartup<H2020.IPMDecisions.UPR.API.Startup>();
                  webBuilder
                        .UseTestServer()
                        .UseConfiguration(configuration);
              })
              .StartAsync();

            using (var scope = _host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ApplicationDbContext>();
                if (tempDatabase != null)
                    context.Database.EnsureCreated();
            }
        }

        public async Task DisposeAsync()
        {
            await _host.StopAsync();
        }

        public void Dispose()
        {
            tempDatabase.Drop();
            _host.Dispose();
        }
    }
}