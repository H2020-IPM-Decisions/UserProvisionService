using System;
using DoomedDatabases.Postgres;
using H2020.IPMDecisions.UPR.Data.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public ApplicationDbContext DbContext { get; }
        private readonly ITestDatabase tempDatabase;

        public DatabaseFixture()
        {            
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();

            // Remember to create integration_test_user in PostgreSQL. User need to be able to create DB
            //  e.g: CREATE USER yourUsername WITH PASSWORD 'yourPassword' CREATEDB;
            var connectionString = configuration["ConnectionStrings:MyPostgreSQLConnection"];

            tempDatabase = new TestDatabaseBuilder()
                .WithConnectionString(connectionString)
                .Build();

            tempDatabase.Create();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseNpgsql(tempDatabase.ConnectionString);
            DbContext = new ApplicationDbContext(builder.Options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            tempDatabase.Drop();
        }
    }

    [CollectionDefinition("Database")]
    public class DatabaseCollectionFixture : ICollectionFixture<DatabaseFixture>
    {
    }
}