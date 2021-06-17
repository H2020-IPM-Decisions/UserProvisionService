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
    public class FarmContextTests
    {
        [Fact]
        public async Task AddNewFarm_WithUserProfileSameTime_True()
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

                // Act

                context.UserProfile.Add(userProfile);
                var dbEntries = context.SaveChanges();

                // Assert
                var userProfileObject = await context
                   .UserProfile
                   .Include(u => u.UserAddress)
                   .Include(u => u.UserFarms)
                   .SingleOrDefaultAsync(u =>
                   u.UserId == userProfile.UserId);

                Assert.Equal(5, dbEntries);
                userProfileObject.UserAddress.Should().BeNull();
                userProfileObject.UserFarms.Should().NotBeNull();
                userProfileObject.UserFarms.Count.Should().Equals(1);

            }
        }

        [Fact]
        public async Task AddNewFarm_UserProfileAlreadyExist_True()
        {
            // Arrange
            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

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
                    Location = new Point(1, 1),
                    WeatherForecast = weatherForecast,
                    WeatherHistorical = weatherHistorical
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1"
                };

                var userFarmType = context.UserFarmType.FirstOrDefault(u => u.Description.Equals("Owner"));

                // Act

                context.UserProfile.Add(userProfile);
                context.SaveChanges();


                var userProfileFromDb = await context
                  .UserProfile
                  .SingleOrDefaultAsync(u =>
                  u.UserId == userProfile.UserId);

                userProfile.UserFarms = new List<UserFarm>{
                    new UserFarm
                    {
                        UserProfile = userProfileFromDb,
                        Farm = farm,
                        UserFarmType = userFarmType
                    }
                };

                var dbEntries = context.SaveChanges();

                // Assert
                var userProfileObject = await context
                   .UserProfile
                   .Include(u => u.UserAddress)
                   .Include(u => u.UserFarms)
                   .SingleOrDefaultAsync(u =>
                   u.UserId == userProfile.UserId);

                Assert.Equal(2, dbEntries);
                userProfileObject.UserAddress.Should().BeNull();
                userProfileObject.UserFarms.Should().NotBeNull();
                userProfileObject.UserFarms.Count.Should().Be(1);

            }
        }

        [Fact]
        public async Task AddTwoFarm_UserProfileAlreadyExist_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

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
                    Location = new Point(1, 1),
                    WeatherForecast = weatherForecast,
                    WeatherHistorical = weatherHistorical
                };

                var farm1 = new Farm()
                {
                    Name = "My Second Farm",
                    Location = new Point(2, 2),
                    WeatherForecast = weatherForecast,
                    WeatherHistorical = weatherHistorical
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1"
                };

                var userFarmType = context.UserFarmType.FirstOrDefault(u => u.Description.Equals("Owner"));

                // Act

                context.UserProfile.Add(userProfile);
                context.SaveChanges();


                var userProfileFromDb = await context
                  .UserProfile
                  .SingleOrDefaultAsync(u =>
                  u.UserId == userProfile.UserId);

                userProfileFromDb.UserFarms = new List<UserFarm>{
                    new UserFarm
                    {
                        UserProfile = userProfileFromDb,
                        Farm = farm,
                        UserFarmType = userFarmType
                    },
                    new UserFarm
                    {
                        UserProfile = userProfileFromDb,
                        Farm = farm1,
                        UserFarmType = userFarmType
                    }
                };

                var dbEntries = context.SaveChanges();

                // Assert
                var userProfileObject = await context
                   .UserProfile
                   .Include(u => u.UserAddress)
                   .Include(u => u.UserFarms)
                   .SingleOrDefaultAsync(u =>
                   u.UserId == userProfile.UserId);

                Assert.Equal(4, dbEntries);
                userProfileObject.UserAddress.Should().BeNull();
                userProfileObject.UserFarms.Should().NotBeNull();
                userProfileObject.UserFarms.Count.Should().Be(2);
            }
        }

        [Fact]
        public async Task AddUserProfileToExistingFarm_FarmAlreadyExist_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

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
                    Location = new Point(1, 1),
                    WeatherForecast = weatherForecast,
                    WeatherHistorical = weatherHistorical
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "AAA"
                };

                var newUserProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "BBB"
                };

                var userFarmType = context.UserFarmType.FirstOrDefault(u => u.Description.Equals("Owner"));

                // Act

                context.UserProfile.Add(userProfile);

                userProfile.UserFarms = new List<UserFarm>{
                    new UserFarm
                    {
                        UserProfile = userProfile,
                        Farm = farm,
                        UserFarmType = userFarmType
                    }
                };

                context.SaveChanges();


                var farmFromDb = await context
                  .Farm
                  .SingleOrDefaultAsync(f =>
                  f.Id == farm.Id);


                farmFromDb.UserFarms.Add(
                    new UserFarm
                    {
                        UserProfile = newUserProfile,
                        Farm = farmFromDb,
                        UserFarmType = userFarmType
                    }
                );

                var dbEntries = context.SaveChanges();

                // Assert
                var farmObject = await context
                  .Farm
                  .Include(u => u.UserFarms)
                  .ThenInclude(uf => uf.UserProfile)
                  .SingleOrDefaultAsync(f =>
                  f.Id == farmFromDb.Id);

                Assert.Equal(2, dbEntries);
                farmObject.UserFarms.Should().NotBeNull();
                farmObject.UserFarms.Count.Should().Be(2);

                var usersInOrder = farmObject.UserFarms.OrderBy(u => u.UserProfile.FirstName).ToList();
                usersInOrder.First().UserProfile.FirstName.Should().Be("AAA");
                usersInOrder.Last().UserProfile.FirstName.Should().Be("BBB");
            }
        }

    }
}
