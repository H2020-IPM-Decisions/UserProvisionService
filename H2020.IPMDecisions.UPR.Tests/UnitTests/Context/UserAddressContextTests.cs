using FluentAssertions;
using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.UnitTests.Context
{
    [Collection("WithDatabase")]
    [Trait("Category", "Docker")]
    public class UserAddressContextTests
    {

        [Fact]
        public async Task AddNewAddress_WithUserProfileSameTime_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var userAddress = new UserAddress()
                {
                    Street = "New Street"
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1",
                    UserAddress = userAddress
                };

                // Act

                context.UserProfile.Add(userProfile);
                var dbEntries = context.SaveChanges();

                // Assert
                var userProfileObject = await context
                   .UserProfile
                   .Include(u => u.UserAddress)
                   .SingleOrDefaultAsync(u =>
                   u.UserId == userProfile.UserId);

                Assert.Equal(2, dbEntries);
                userProfileObject.UserAddress.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task AddNewAddress_AfterUserProfileCreated_True()
        {
            // Arrange

            using (var databaseFixture = new DatabaseFixture())
            {
                var context = databaseFixture.DbContext;

                var street = "New Street";
                var userAddress = new UserAddress()
                {
                    Street = street
                };

                var userProfile = new UserProfile()
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "1"
                };

                // Act

                context.UserProfile.Add(userProfile);
                context.SaveChanges();

                var userProfileFromDb = await context
                  .UserProfile
                  .SingleOrDefaultAsync(u =>
                  u.UserId == userProfile.UserId);


                userProfileFromDb.UserAddress = userAddress;
                var dbEntries = context.SaveChanges();

                // Assert
                var userProfileObject = await context
                   .UserProfile
                   .SingleOrDefaultAsync(u =>
                   u.UserId == userProfile.UserId);

                Assert.Equal(2, dbEntries);
                userProfileObject.UserAddress.Should().NotBeNull();
                userProfileObject.UserAddress.Street.Should().Be(street);
            }
        }
    }
}
