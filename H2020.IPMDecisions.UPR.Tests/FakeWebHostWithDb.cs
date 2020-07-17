using DoomedDatabases.Postgres;
using H2020.IPMDecisions.UPR.API;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests
{
    public class FakeWebHostWithDb : IAsyncLifetime
    {
        public IHost Host;
        private ITestDatabase tempDatabase;
        public bool IsDatabaseInitialized;
        private ApplicationDbContext _context;

        public readonly string DefaultAdminUserEmail = "admin@test.com";
        public readonly string DefaultNormalUserEmail = "user@test.com";
        public readonly Guid DefaultAdminUserId = Guid.Parse("380f0a69-a009-4c34-8496-9a43c2e069ba");
        public readonly Guid DefaultNormalUserId = Guid.Parse("89f4cb8a-c803-11ea-87d0-0242ac130003");
        public readonly Guid ExtraNormalUserId = Guid.Parse("68840b5c-803b-461e-a262-cdb9932d203b");

        [Trait("Category", "Docker")]
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

            string databaseGuid = Guid.NewGuid().ToString();
            var newConString = string.Format("{0}{1}", tempDatabase.ConnectionString.ToString(), databaseGuid);
            configuration["ConnectionStrings:MyPostgreSQLConnection"] = newConString;

            Host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder
                    .UseEnvironment(Environments.Staging)
                    .UseStartup<Startup>()
                    .UseTestServer()
                    .UseConfiguration(configuration);
              })
              .StartAsync();

            using (var scope = Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                _context = services.GetService<ApplicationDbContext>();
                if (!IsDatabaseInitialized)
                    Seed();
            }
        }

        public async Task DisposeAsync()
        {
            using (var scope = Host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                _context = services.GetService<ApplicationDbContext>();
                _context.Database.EnsureDeleted();
            }

            if (tempDatabase != null)
                tempDatabase.Drop();
            await Host.StopAsync();
            Host.Dispose();
        }

        private void Seed()
        {
            if (!IsDatabaseInitialized)
            {
                using (_context)
                {
                    _context.Database.EnsureDeleted();
                    _context.Database.EnsureCreated();

                    IList<UserProfile> defaultUsers = new List<UserProfile>()
                    {
                        new UserProfile()
                        {
                            UserId = DefaultAdminUserId,
                            FirstName = "Admin"
                        },
                        new UserProfile()
                        {
                            UserId = DefaultNormalUserId,
                            FirstName = "Default"
                        },
                        new UserProfile()
                        {
                            UserId = ExtraNormalUserId,
                            FirstName = "Extra"
                        },
                    };

                    _context.UserProfile.AddRange(defaultUsers);
                    _context.SaveChanges();
                }
                IsDatabaseInitialized = true;
            }
        }

        [CollectionDefinition("FakeWebHostWithDb")]
        public class FakeWebHostWithDbCollectionFixture : ICollectionFixture<FakeWebHostWithDb>
        {
        }
    }
}