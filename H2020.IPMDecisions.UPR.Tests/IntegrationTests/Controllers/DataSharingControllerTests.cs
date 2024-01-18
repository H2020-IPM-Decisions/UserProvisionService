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
    public class DataSharingControllerTests : FakeApiGatewayHost
    {
        private FakeWebHostWithDb fakeWebHost;

        public DataSharingControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void Post_NoClaims_Forbibben()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = Guid.NewGuid();
            var myUserToken = TokenGeneratorTests.GenerateToken(userId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("email", "a@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async void Post_FarmerClaim_Forbibben()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","farmer")
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
            jsonObject.Add("email", "a@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async void Post_BadAdvisorClaim_Forbibben()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("badText","advisor")
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
            jsonObject.Add("email", "a@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async void Post_BadAdvisorClaimCapitalA_Forbibben()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("useraccesstype","Advisor")
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
            jsonObject.Add("email", "a@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async void Post_FarmerEmailDoNotExistInIDP_BadRequest()
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
            jsonObject.Add("email", "idonotexist@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("User requested not in the system.");
        }

        [Fact]
        public async void Post_FarmerProfileDoNotExist_BadRequest()
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
            jsonObject.Add("email", "usernoprofile@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("User requested do not have a profile in the system.");
        }

        [Fact]
        public async void Post_AdvisorCanNotRequestOwnData_BadRequest()
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

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("email", "advisor@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("User can not request its own data.");
        }

        [Fact]
        public async void Post_AdvisorProfileDoNotExist_BadRequest()
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
            jsonObject.Add("email", "defaultnormaluserid@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("User requesting data share do not have a profile in the system.");
        }

        [Fact]
        public async void Post_FarmerDeclinedDataShareRequest_BadRequest()
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

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("email", "dataShareRequestDeclined@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("The user has declined access to the data.");
        }

        [Fact]
        public async void Post_DataShareRequestAlreadyOnSystem_BadRequest()
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

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("email", "existingDataShareRequest@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("You have already requested data to this user.");
        }

        [Fact]
        public async void Post_EmailServiceDown_BadRequest()
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

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("email", "nosendemail@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseContent.Should().Contain("Request created but error sending email.");
        }

        [Fact]
        public async void Post_RequestCreated_Ok()
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

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObject = new JsonObject();
            jsonObject.Add("email", "defaultnormaluserid@z.com");
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/datashare", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}