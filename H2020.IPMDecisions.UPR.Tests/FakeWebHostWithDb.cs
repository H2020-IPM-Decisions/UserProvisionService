using DoomedDatabases.Postgres;
using H2020.IPMDecisions.UPR.API;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly Guid DefaultFarmId = Guid.Parse("16bddb81-0ee6-492d-8172-cddf8855f683");
        public readonly Guid UserWith3FarmsId = Guid.Parse("f5497c01-1375-46ff-a8bc-b4640a8c84cd");
        public readonly Guid UserWith2FarmsId = Guid.Parse("23482647-f8ae-49e0-881e-93aeead00ac7");
        public readonly string DefaultFarmName = "New Farm";
        public readonly Guid FirstFarmIdUser2Farms = Guid.Parse("843185ed-5f66-4982-a6e6-38379c39fe92");

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

            configuration["ConnectionStrings:MyPostgreSQLConnection"] = tempDatabase.ConnectionString.ToString();

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

                    var userFarmType = _context.UserFarmType.FirstOrDefault(u => u.Description.Equals("Owner"));


                    IList<UserProfile> defaultUsers = new List<UserProfile>()
                    {
                        new UserProfile()
                        {
                            UserId = DefaultAdminUserId,
                            FirstName = "Admin"
                        },
                        new UserProfile()
                        {
                            UserId = ExtraNormalUserId,
                            FirstName = "Extra"
                        }
                    };

                    _context.UserProfile.AddRange(defaultUsers);

                    var userWithFarm = new UserProfile()
                    {
                        UserId = DefaultNormalUserId,
                        FirstName = "Default"
                    };

                    userWithFarm.UserFarms = new List<UserFarm>
                    {
                        new UserFarm
                        {
                            UserProfile = userWithFarm,
                            Farm = new Farm {
                                Id = DefaultFarmId,
                                Name = DefaultFarmName,
                                Location = new Point(1, 1)
                            },
                            UserFarmType = userFarmType
                        }
                    };

                    _context.UserProfile.Add(userWithFarm);

                    var userWith2Farms = new UserProfile()
                    {
                        UserId = UserWith2FarmsId,
                        FirstName = "FewFarms"
                    };

                    userWith2Farms.UserFarms = new List<UserFarm>
                    {
                        new UserFarm
                        {
                            UserProfile = userWith2Farms,
                            Farm = new Farm {
                                Id = FirstFarmIdUser2Farms,
                                Name = "AAA",
                                Location = new Point(11, 10)
                            },
                            UserFarmType = userFarmType
                        },
                        new UserFarm
                        {
                            UserProfile = userWith2Farms,
                            Farm = new Farm {
                                Name = "BBB",
                                Location = new Point(22, 20)
                            },
                            UserFarmType = userFarmType
                        }
                    };

                    _context.UserProfile.Add(userWith2Farms);

                    var userWith3Farms = new UserProfile()
                    {
                        UserId = UserWith3FarmsId,
                        FirstName = "FewFarms"
                    };

                    userWith3Farms.UserFarms = new List<UserFarm>
                    {
                        new UserFarm
                        {
                            UserProfile = userWith3Farms,
                            Farm = new Farm {
                                Name = "AAA",
                                Location = new Point(10, 10)
                            },
                            UserFarmType = userFarmType
                        },
                        new UserFarm
                        {
                            UserProfile = userWith3Farms,
                            Farm = new Farm {
                                Name = "BBB",
                                Location = new Point(20, 20)
                            },
                            UserFarmType = userFarmType
                        },
                        new UserFarm
                        {
                            UserProfile = userWith3Farms,
                            Farm = new Farm {
                                Name = "ZZZ",
                                Location = new Point(30, 30)
                            },
                            UserFarmType = userFarmType
                        }
                    };

                    _context.UserProfile.Add(userWith3Farms);

                    _context.SaveChanges();
                };
            }
            IsDatabaseInitialized = true;
        }
    }

    [CollectionDefinition("FakeWebHostWithDb")]
    public class FakeWebHostWithDbCollectionFixture : ICollectionFixture<FakeWebHostWithDb>
    {
    }
}