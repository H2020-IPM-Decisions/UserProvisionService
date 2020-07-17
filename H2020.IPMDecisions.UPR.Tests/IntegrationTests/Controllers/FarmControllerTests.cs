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
        private string myAdminUserToken;
        private string myDefaultUserToken;
        private string myExtraUserToken;

        public FarmControllerTests(FakeWebHostWithDb fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
            myAdminUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultAdminUserId, "admin");
            myDefaultUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);
            myExtraUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.ExtraNormalUserId);
        }

        [Fact]
        public async void PostNewFarmToUser_ValidCall_OkFarmDto()
        {
            // Arrange

            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

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
            jsonObject.Add("name", "new farm");
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Name.Should().Be("new farm");
            responseDeserialized.Location.X.Should().Be(1);
        }
    }
}