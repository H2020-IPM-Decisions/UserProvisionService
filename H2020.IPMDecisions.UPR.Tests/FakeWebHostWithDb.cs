using System.Net.Http;
using System.Threading.Tasks;
using DoomedDatabases.Postgres;
using H2020.IPMDecisions.UPR.API;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests
{
    public class FakeWebHostWithDb : IAsyncLifetime
    {
        public IHost Host;
        private ITestDatabase tempDatabase;

        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.Test.json")
              .Build();

            // Remember to create integration_test_user in PostgreSQL. User need to be able to create DB
            // Get into docker container: docker exec -it {ContainerID} psql -U {adminUser} postgres
            //  e.g: CREATE USER yourUsername WITH PASSWORD 'yourPassword' CREATEDB;
            var connectionString = configuration["ConnectionStrings:MyPostgreSQLConnection"];

            tempDatabase = new TestDatabaseBuilder()
            .WithConnectionString(connectionString)
            .Build();
            tempDatabase.Create();

            if (tempDatabase != null)
                configuration["ConnectionStrings:MyPostgreSQLConnection"] = tempDatabase.ConnectionString.ToString();

            Host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder
                    .UseEnvironment(Microsoft.Extensions.Hosting.Environments.Staging)
                    .UseStartup<Startup>()
                    .UseTestServer()
                    .UseConfiguration(configuration);
              })
              .StartAsync();

            using (var scope = Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<ApplicationDbContext>();
                if (tempDatabase != null)
                    context.Database.EnsureCreated();
            }
        }

        public async Task DisposeAsync()
        {
            if (tempDatabase != null)
                tempDatabase.Drop();
            await Host.StopAsync();
            Host.Dispose();
        }

        [CollectionDefinition("FakeWebHostWithDb")]
        public class FakeWebHostWithDbCollectionFixture : ICollectionFixture<FakeWebHostWithDb>
        {
        }
    }
}