using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.UnitTests.Context
{
    [Collection("WithDatabase")]
    [Trait("Category", "Docker")]
    public class UserProfileContextTests
    {

        [Fact]
        public void AddNewProfile_OnlyProfile_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1"
                };

                // Act

                context.UserProfile.Add(userProfile);
                var dbEntries = context.SaveChanges();

                // Assert

                Assert.Equal(1, dbEntries);
            }
        }


        [Fact]
        public async Task AddNewProfile_NoFirstName_Exception()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid()
                };

                // Act
                context.UserProfile.Add(userProfile);
                async Task action() => await context.SaveChangesAsync();

                // Assert
                var exception = await Assert.ThrowsAsync<DbUpdateException>(action);
                Assert.Contains("null value in column", exception.InnerException.Message);
            }
        }

        [Fact]
        public async Task AddNewProfile_ProfileDuplicateId_Exception()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var profileId = Guid.NewGuid();

                var userProfile = new UserProfile()
                {
                    UserId = profileId,
                    FirstName = "Name"
                };

                var userProfileDuplicated = new UserProfile()
                {
                    UserId = profileId,
                    FirstName = "Other Name"
                };

                // Act

                await context.UserProfile.AddAsync(userProfile);
                async Task action() => await context.UserProfile.AddAsync(userProfileDuplicated);

                // Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
                Assert.Contains("instance with the same key value for {'UserId'} is already being tracked", exception.Message);
            }
        }


        [Fact]
        public async Task AddNewProfile_DuplicateUserId_Exception()
        {
            // Arange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var userId = Guid.NewGuid();

                var userProfile = new UserProfile()
                {
                    UserId = userId,
                    FirstName = "Name"
                };

                var userProfileDuplicated = new UserProfile()
                {
                    UserId = userId,
                    FirstName = "Other Name"
                };

                // Act

                await context.UserProfile.AddAsync(userProfile);
                async Task action() => await context.UserProfile.AddAsync(userProfileDuplicated);

                // Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
                Assert.Contains("instance with the same key value for {'UserId'} is already being tracked", exception.Message);
            }
        }
    }
}
