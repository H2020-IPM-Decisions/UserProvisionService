using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    [Collection("FakeWebHostWithDb")]
    [Trait("Category", "Docker")]
    public class DataSharingControllerFarmerTests : FakeApiGatewayHost
    {
        private FakeWebHostWithDb fakeWebHost;

        public DataSharingControllerFarmerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void Delete_FarmerRequest_NoContent()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = fakeWebHost.UserWith3FarmsId;
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","farmer")
            };
            var myUserToken = TokenGeneratorTests.GenerateToken(userId, userClaims: claims);

            httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myUserToken);

            var requestId = fakeWebHost.DataShareIdToDelete1;

            // Act
            var response = await httpClient.DeleteAsync(string.Format("api/datashare/{0}", requestId));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async void Delete_AdvisorRequest_NoContent()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = fakeWebHost.DefaultAdvisorUserId;
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","advisor")
            };
            var myUserToken = TokenGeneratorTests.GenerateToken(userId, userClaims: claims);

            httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myUserToken);

            var requestId = fakeWebHost.DataShareIdToDelete2;

            // Act
            var response = await httpClient.DeleteAsync(string.Format("api/datashare/{0}", requestId));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async void Delete_RequestDoNotBelongAdvisor_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = fakeWebHost.DefaultAdvisorUserId;
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","advisor")
            };
            var myUserToken = TokenGeneratorTests.GenerateToken(userId, userClaims: claims);

            httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myUserToken);

            var requestId = fakeWebHost.DefaultDataShareId;

            // Act
            var response = await httpClient.DeleteAsync(string.Format("api/datashare/{0}", requestId));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Request do not belong to the user.");
        }

        [Fact]
        public async void Delete_RequestDoNotBelongFarmer_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = fakeWebHost.DefaultNormalUserId;
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","farmer")
            };
            var myUserToken = TokenGeneratorTests.GenerateToken(userId, userClaims: claims);

            httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myUserToken);

            var requestId = fakeWebHost.DefaultDataShareId;

            // Act
            var response = await httpClient.DeleteAsync(string.Format("api/datashare/{0}", requestId));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Request do not belong to the user.");
        }

        [Fact]
        public async void Delete_RequestDoNotExist_BadRequest()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = fakeWebHost.DefaultNormalUserId;
            var myUserToken = TokenGeneratorTests.GenerateToken(userId);

            httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myUserToken);

            var requestId = Guid.NewGuid();

            // Act
            var response = await httpClient.DeleteAsync(string.Format("api/datashare/{0}", requestId));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Request do not belong to the user.");
        }

        [Fact]
        public async void PostReply_Advisor_Forbibben()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","advisor")
            };
            var myUserToken = TokenGeneratorTests.GenerateToken(userId, userClaims: claims);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("requesterId", Guid.NewGuid().ToString());
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare/reply", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async void PostUpdate_Advisor_Forbibben()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","advisor")
            };
            var myUserToken = TokenGeneratorTests.GenerateToken(userId, userClaims: claims);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("requesterId", Guid.NewGuid().ToString());
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare/update", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        } 
    }
}