using FluentAssertions;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    [Collection("FakeWebHostWithDb")]
    [Trait("Category", "Docker")]
    public class UserProfilesControllerTests : IClassFixture<FakeWebHostWithDb>
    {
        private FakeWebHostWithDb fakeWebHost;
        private string myAdminToken;
        public UserProfilesControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;

            myAdminToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultAdminUserId, "Admin");
        }

        #region Admin Role
        [Fact]
        public async void Post_AdminCreatesOtherProfile_Created()
        {
            // Arrange
            var resourceUserId = Guid.NewGuid();

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles?userid={0}", resourceUserId), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized.UserId.Should().Be(resourceUserId);
            responseDeserialized.FullName.Should().Be("SomeName SomeLastNameName");
        }

        [Fact]
        public async void Post_AdminCreatesOtherProfileDefaultGuid_AdminProfileAlreadyExist()
        {
            // Arrange
            var resourceUserId = new Guid();
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles?userid={0}", resourceUserId), content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("User profile aready exits.");
        }

        [Fact]
        public async void Post_AdminCreatesProfileNoGuid_AdminProfileAlreadyExist()
        {
            // Arrange
            var newUserId = Guid.NewGuid();
            var myNewAdminToken = TokenGeneratorTests.GenerateToken(newUserId, "Admin");

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myNewAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles"), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized.UserId.Should().Be(newUserId);
            responseDeserialized.FullName.Should().Be("SomeName SomeLastNameName");
        }

        [Fact]
        public async void Post_AdminCreatesProfileDefaultGuid_Created()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles"), content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("User profile aready exits.");
        }

        [Fact]
        public async void Get_AdminUsesUnexistingUserId_NotFound()
        {
            // Arrange
            var resourceUserId = Guid.NewGuid();
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync(string.Format("api/users/profiles?userid={0}", resourceUserId));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void Delete_AdminUsesUnexistingUserId_NotContent()
        {
            // Arrange
            var resourceUserId = Guid.NewGuid();
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.DeleteAsync(string.Format("api/users/profiles?userid={0}", resourceUserId));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async void Patch_AdminUsesUnexistingUserId_CreatesNewUserProfile()
        {
            // Arrange
            var resourceUserId = Guid.NewGuid();
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var patchDoc = new JsonPatchDocument<UserProfileForUpdateDto>();
            patchDoc.Add(u => u.FirstName, "NewName");
            var serializedPatchDoc = JsonConvert.SerializeObject(patchDoc);
            var content = new StringContent(
                serializedPatchDoc,
                Encoding.UTF8,
                "application/json-patch+json");

            // Act
            var response = await httpClient.PatchAsync(string.Format("api/users/profiles?userid={0}", resourceUserId), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized.UserId.Should().Be(resourceUserId);
            responseDeserialized.FullName.Should().Be("NewName");
        }

        [Fact]
        public async void Patch_AdminUsesUnexistingUserIdButNoFirstName_BadRequest()
        {
            // Arrange
            var resourceUserId = Guid.NewGuid();
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var patchDoc = new JsonPatchDocument<UserProfileForUpdateDto>();
            patchDoc.Add(u => u.LastName, "NewLastNameName");
            var serializedPatchDoc = JsonConvert.SerializeObject(patchDoc);
            var content = new StringContent(
                serializedPatchDoc,
                Encoding.UTF8,
                "application/json-patch+json");

            // Act
            var response = await httpClient.PatchAsync(string.Format("api/users/profiles?userid={0}", resourceUserId), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseDeserialized.Title.Should().Be("One or more validation errors occurred.");
        }
        #endregion

        #region Normal user
        [Fact]
        public async void Post_CreateProfile_Created()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var myToken = TokenGeneratorTests.GenerateToken(userId);

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles"), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized.UserId.Should().Be(userId);
            responseDeserialized.FullName.Should().Be("SomeName SomeLastNameName");
        }

        [Fact]
        public async void Post_CreateProfile_CreatedUserProfileFull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var myToken = TokenGeneratorTests.GenerateToken(userId);

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/vnd.h2020ipmdecisions.profile.full+json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            jsonObject.Add("street", "Main Street");
            jsonObject.Add("city", "London");
            jsonObject.Add("postcode", "AB12CD");
            jsonObject.Add("country", "UK");

            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles"), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileFullDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized.UserId.Should().Be(userId);
            responseDeserialized.FirstName.Should().Be("SomeName");
            responseDeserialized.LastName.Should().Be("SomeLastNameName");
            responseDeserialized.Street.Should().Be("Main Street");
            responseDeserialized.City.Should().Be("London");
            responseDeserialized.Postcode.Should().Be("AB12CD");
            responseDeserialized.Country.Should().Be("UK");
        }

        [Fact]
        public async void Post_CreateProfile_CreatedUserProfileFullHATEOAS()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var myToken = TokenGeneratorTests.GenerateToken(userId);

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/vnd.h2020ipmdecisions.profile.full.hateoas+json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            jsonObject.Add("street", "Main Street");
            jsonObject.Add("city", "London");
            jsonObject.Add("postcode", "AB12CD");
            jsonObject.Add("country", "UK");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync(string.Format("api/users/profiles"), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<IDictionary<string, object>>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized["userId"].Should().Be(userId.ToString());
            responseDeserialized["firstName"].Should().Be("SomeName");
            responseDeserialized["lastName"].Should().Be("SomeLastNameName");
            responseDeserialized.ContainsKey("links").Should().BeTrue();
        }

        [Fact]
        public async void Delete_UserId_NotContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var myToken = TokenGeneratorTests.GenerateToken(userId);

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            jsonObject.Add("lastName", "SomeLastNameName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");
            await httpClient.PostAsync("api/users/profiles", content);

            // Act
            var response = await httpClient.DeleteAsync("api/users/profiles");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async void Patch_UserChangesName_ChangesProfile()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);
            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myToken);

            httpClient
               .DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var patchDoc = new JsonPatchDocument<UserProfileForUpdateDto>();
            patchDoc.Add(u => u.FirstName, "NewName");
            var serializedPatchDoc = JsonConvert.SerializeObject(patchDoc);
            var content = new StringContent(
                serializedPatchDoc,
                Encoding.UTF8,
                "application/json-patch+json");

            // Act
            var response = await httpClient.PatchAsync("api/users/profiles", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        #endregion
    }
}