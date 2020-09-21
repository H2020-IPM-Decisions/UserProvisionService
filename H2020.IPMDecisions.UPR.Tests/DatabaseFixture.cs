using DoomedDatabases.Postgres;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace H2020.IPMDecisions.UPR.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public ApplicationDbContext DbContext { get; }
        private ITestDatabase tempDatabase;

        public DatabaseFixture()
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

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseNpgsql(tempDatabase.ConnectionString
                , b => b.UseNetTopologySuite());

            DbContext = new ApplicationDbContext(builder.Options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            if (tempDatabase != null)
                tempDatabase.Drop();
        }
    }
}