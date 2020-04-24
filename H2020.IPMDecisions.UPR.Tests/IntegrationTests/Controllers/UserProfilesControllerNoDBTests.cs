using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Json;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    [Collection("FakeWebHost")]
    public class UserProfilesControllerNoDBTests
    {
        private readonly FakeWebHost fakeWebHost;

        public UserProfilesControllerNoDBTests(FakeWebHost fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }      

        [Fact]
        public async void Get_DifferentUserIdUrlAndToken_Unauthorized()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var resourceUserId = Guid.NewGuid();

            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId); 

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await fakeWebHost.httpClient.GetAsync(string.Format("api/users/{0}/profiles", resourceUserId));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async void Post_DifferentUserIdUrlAndToken_Unauthorized()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var resourceUserId = Guid.NewGuid();

            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);

            fakeWebHost.
                httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            var content = new StringContent(
                jsonObject.ToString(), 
                Encoding.UTF8, 
                "application/json");

            // Act
            var response = await fakeWebHost.httpClient.PostAsync(string.Format("api/users/{0}/profiles", resourceUserId),content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async void Post_WrongContentTypeHeader_UnsupportedMediaType()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/invalidContentType");

            // Act
            var response = await fakeWebHost.httpClient.PostAsync(string.Format("api/users/{0}/profiles", tokenUserId), content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async void Delete_DifferentUserIdUrlAndToken_Unauthorized()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var resourceUserId = Guid.NewGuid();

            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await fakeWebHost.httpClient.DeleteAsync(string.Format("api/users/{0}/profiles", resourceUserId));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async void Put_DifferentUserIdUrlAndToken_Unauthorized()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var resourceUserId = Guid.NewGuid();

            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            var content = new StringContent(
                jsonObject.ToString(), 
                Encoding.UTF8, 
                "application/json-patch+json");

            // Act
            var response = await fakeWebHost.httpClient.PatchAsync(string.Format("api/users/{0}/profiles", resourceUserId), content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async void Put_WrongContentTypeHeader_UnsupportedMediaType()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);

            fakeWebHost
                .httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("firstName", "SomeName");
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/shouldbepatch");

            // Act
            var response = await fakeWebHost.httpClient.PatchAsync(string.Format("api/users/{0}/profiles", tokenUserId), content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }
    }
}