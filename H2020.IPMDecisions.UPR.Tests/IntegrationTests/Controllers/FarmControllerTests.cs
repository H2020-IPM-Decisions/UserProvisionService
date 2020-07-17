using System;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    [Trait("Category", "Docker")]
    public class FarmControllerTests: IClassFixture<FakeWebHostWithDb>
    {
        private FakeWebHostWithDb fakeWebHost;

        public FarmControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void PostNewFarmToUser_ValidCall_OkFarmDto()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myDefaultUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myDefaultUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonObjectLocation = new JsonObject();
            jsonObjectLocation.Add("x", "1");
            jsonObjectLocation.Add("y", "1");
            jsonObjectLocation.Add("srid", "4236");

            var jsonObject = new JsonObject();
            jsonObject.Add("name", fakeWebHost.DefaultFarmName);
            jsonObject.Add("location", jsonObjectLocation);
            var content = new StringContent(
                jsonObject.ToString(),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await httpClient.PostAsync("api/farms", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<FarmDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            responseDeserialized.Name.Should().Be(fakeWebHost.DefaultFarmName);
            responseDeserialized.Location.X.Should().Be(1);
        }


        [Fact]
        public async void GetFarm_UserGetsOwnFarm_OkFarmDto()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myDefaultUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myDefaultUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync(string.Format("api/farms/{0}", fakeWebHost.DefaultFarmId.ToString()));
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<FarmDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Name.Should().Be(fakeWebHost.DefaultFarmName);
        }

        [Fact]
        public async void GetFarm_FarmDontExist_NotFound()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myDefaultUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myDefaultUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var randomFarmId = Guid.NewGuid();

            // Act
            var response = await httpClient.GetAsync(string.Format("api/farms/{0}", randomFarmId));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void GetFarm_FarmExistButUserDontOwnFarm_NotFound()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myExtraUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.ExtraNormalUserId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myExtraUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync(string.Format("api/farms/{0}", fakeWebHost.DefaultFarmId.ToString()));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void AdminGetFarm_UserDontOwnFarm_OK()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myAdminUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultAdminUserId, "admin");    

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myAdminUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync(string.Format("api/farms/{0}", fakeWebHost.DefaultFarmId.ToString()));
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<FarmDto>(responseContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Name.Should().Be(fakeWebHost.DefaultFarmName);
        }
    }
}