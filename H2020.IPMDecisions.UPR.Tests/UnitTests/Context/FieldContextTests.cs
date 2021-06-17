using FluentAssertions;
using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.UnitTests.Context
{
    [Collection("WithDatabase")]
    [Trait("Category", "Docker")]
    public class FieldContextTests
    {
        [Fact]
        public async Task AddNewField_ExistingFarm_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var location = new Point(51.5, -0.12);
                var weatherForecast = new WeatherForecast()
                {
                    Name = "1",
                    Url = "1",
                    WeatherId = "1"
                };
                context.WeatherForecast.Add(weatherForecast);
                var weatherHistorical = new WeatherHistorical()
                {
                    Name = "1",
                    Url = "1",
                    WeatherId = "1"
                };
                context.WeatherHistorical.Add(weatherHistorical);

                var farm = new Farm()
                {
                    Name = "My Farm",
                    Location = location,
                    WeatherForecast = weatherForecast,
                    WeatherHistorical = weatherHistorical
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1"
                };

                var userFarmType = context.UserFarmType.FirstOrDefault(u => u.Description.Equals("Owner"));

                userProfile.UserFarms = new List<UserFarm>{
                    new UserFarm
                    {
                        UserProfile = userProfile,
                        Farm = farm,
                        UserFarmType = userFarmType
                    }
                };

                context.UserProfile.Add(userProfile);
                context.SaveChanges();


                // Act
                var field = new Field()
                {
                    Name = "My Farm",
                    FarmId = farm.Id
                };

                context.Field.Add(field);
                context.SaveChanges();

                // Assert
                var dataFromDb = await context
                   .Farm
                   .Include(f => f.Fields)
                   .SingleOrDefaultAsync(f =>
                   f.Id == farm.Id);

                dataFromDb.Fields.Should().NotBeNull();
                dataFromDb.Fields.Count.Should().Be(1);
            }
        }

        [Fact]
        public async Task AddTwoFields_ExistingFarm_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var location = new Point(51.5, -0.12);
                var weatherForecast = new WeatherForecast()
                {
                    Name = "1",
                    Url = "1",
                    WeatherId = "1"
                };
                context.WeatherForecast.Add(weatherForecast);
                var weatherHistorical = new WeatherHistorical()
                {
                    Name = "1",
                    Url = "1",
                    WeatherId = "1"
                };
                context.WeatherHistorical.Add(weatherHistorical);
                var farm = new Farm()
                {
                    Name = "My Farm",
                    Location = location,
                    WeatherForecast = weatherForecast,
                    WeatherHistorical = weatherHistorical
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1"
                };

                var userFarmType = context.UserFarmType.FirstOrDefault(u => u.Description.Equals("Owner"));

                userProfile.UserFarms = new List<UserFarm>{
                    new UserFarm
                    {
                        UserProfile = userProfile,
                        Farm = farm,
                        UserFarmType = userFarmType
                    }
                };

                context.UserProfile.Add(userProfile);
                context.SaveChanges();


                // Act
                var field = new Field()
                {
                    Name = "My Field",
                    FarmId = farm.Id
                };
                var field2 = new Field()
                {
                    Name = "My Field2",
                    FarmId = farm.Id
                };

                context.Field.Add(field);
                context.Field.Add(field2);
                context.SaveChanges();

                // Assert
                var dataFromDb = await context
                   .Farm
                   .Include(f => f.Fields)
                   .SingleOrDefaultAsync(f =>
                   f.Id == farm.Id);

                dataFromDb.Fields.Should().NotBeNull();
                dataFromDb.Fields.Count.Should().Be(2);
            }
        }

        [Fact]
        public async Task AddField_NoExistingFarm_Exception()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;
                var farmId = Guid.NewGuid();


                // Act
                var field = new Field()
                {
                    Name = "My Field",
                    FarmId = farmId
                };

                context.Field.Add(field);

                async Task action() => await context.SaveChangesAsync();

                // Assert
                var exception = await Assert.ThrowsAsync<DbUpdateException>(action);
                Assert.Contains("violates foreign key constraint", exception.InnerException.Message);
            }
        }
    }
}
