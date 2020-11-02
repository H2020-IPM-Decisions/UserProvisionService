using FluentAssertions;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    [Collection("FakeWebHostWithDb")]
    [Trait("Category", "Docker")]
    public class FarmControllerTests
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
            var myAdminUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultAdminUserId, "Admin");

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

        [Fact]
        public async void UserGetFarms_UserHave3Farms_OK3Farms()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.UserWith3FarmsId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync("api/farms");
            var responseContent = await response.Content.ReadAsStringAsync();

            JObject parsedObject = JObject.Parse(responseContent);
            ShapedDataWithLinks jsonObj = parsedObject.ToObject<ShapedDataWithLinks>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            jsonObj.Value.Count().Should().Be(3);
        }

        [Fact]
        public async void UserGetFarms_NameDesc_OKFarmZZZFirst()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.UserWith3FarmsId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync("api/farms?orderby=name desc");
            var responseContent = await response.Content.ReadAsStringAsync();

            JObject parsedObject = JObject.Parse(responseContent);
            ShapedDataWithLinks jsonObj = parsedObject.ToObject<ShapedDataWithLinks>();
            var firstFarm = jsonObj.Value.FirstOrDefault();
            var lastFarm = jsonObj.Value.LastOrDefault();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            jsonObj.Value.Count().Should().Be(3);
            firstFarm["name"].Should().Be("ZZZ");
            lastFarm["name"].Should().Be("AAA");
        }

        [Fact]
        public async void UserGetFarms_NoFarms_NotFound()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.ExtraNormalUserId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var response = await httpClient.GetAsync("api/farms");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void UserPatch_FarmExist_NotContentAndUpdated()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);
            var newFarmName = "ThisIsANewName";


            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var patchOperation = new JsonObject();
            patchOperation.Add("op", "replace");
            patchOperation.Add("path", "/Name");
            patchOperation.Add("value", newFarmName);

            var patchOperationArray = new JsonArray();
            patchOperationArray.Add(patchOperation);

            var patchContent = new StringContent(
                patchOperationArray.ToString(),
                Encoding.UTF8,
                "application/json-patch+json");

            // Act
            var responsePatch = await httpClient.PatchAsync(string.Format("api/farms/{0}", fakeWebHost.DefaultFarmId), patchContent);
            var responseGet = await httpClient.GetAsync(string.Format("api/farms/{0}", fakeWebHost.DefaultFarmId));
            var responseContent = await responseGet.Content.ReadAsStringAsync();
            var responseDeserialized = JsonConvert.DeserializeObject<FarmDto>(responseContent);

            // Assert
            responsePatch.StatusCode.Should().Be(HttpStatusCode.NoContent);
            responseGet.StatusCode.Should().Be(HttpStatusCode.OK);
            responseDeserialized.Name.Should().Be(newFarmName);
        }

        [Fact]
        public async void UserPatch_FarmDoNotExist_NotFound()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.DefaultNormalUserId);
            var newFarmId = Guid.NewGuid();
            var newFarmName = "ThisIsANewFarm";

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var patchOperation = new JsonObject();
            patchOperation.Add("op", "replace");
            patchOperation.Add("path", "/Name");
            patchOperation.Add("value", newFarmName);

            var jsonObjectLocation = new JsonObject();
            jsonObjectLocation.Add("x", "5");
            jsonObjectLocation.Add("y", "5");
            jsonObjectLocation.Add("srid", "4236");

            var patchOperationLocation = new JsonObject();
            patchOperationLocation.Add("op", "replace");
            patchOperationLocation.Add("path", "/Location");
            patchOperationLocation.Add("value", jsonObjectLocation);

            var patchOperationArray = new JsonArray();
            patchOperationArray.Add(patchOperation);
            patchOperationArray.Add(patchOperationLocation);

            var patchContent = new StringContent(
                patchOperationArray.ToString(),
                Encoding.UTF8,
                "application/json-patch+json");

            // Act
            var responsePatch = await httpClient.PatchAsync(string.Format("api/farms/{0}", newFarmId), patchContent);

            // Assert
            responsePatch.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async void UserDeleteFarm_Has2Farms_NotContentOnly1()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.UserWith2FarmsId);

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
            .DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var responseBeforeDelete = await httpClient.GetAsync("api/farms");
            var responseDelete = await httpClient.DeleteAsync(string.Format("api/farms/{0}", fakeWebHost.FirstFarmIdUser2Farms));
            var responseAfterDelete = await httpClient.GetAsync("api/farms");

            var responseContent = await responseBeforeDelete.Content.ReadAsStringAsync();
            JObject parsedObject = JObject.Parse(responseContent);
            ShapedDataWithLinks dataBeforeDelete = parsedObject.ToObject<ShapedDataWithLinks>();

            responseContent = await responseAfterDelete.Content.ReadAsStringAsync();
            parsedObject = JObject.Parse(responseContent);
            ShapedDataWithLinks dataAfterDelete = parsedObject.ToObject<ShapedDataWithLinks>();

            // Assert
            dataBeforeDelete.Value.Count().Should().Be(2);
            responseDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
            dataAfterDelete.Value.Count().Should().Be(1);
        }

        [Fact]
        public async void UserDeleteFarm_DoNotExist_NotContentSameFarms()
        {
            // Arrange
            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();
            var myUserToken = TokenGeneratorTests.GenerateToken(fakeWebHost.UserWith3FarmsId);
            var randomFarmID = Guid.NewGuid();

            httpClient
                 .DefaultRequestHeaders
                 .Authorization =
                 new AuthenticationHeaderValue("Bearer", myUserToken);

            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            var responseBeforeDelete = await httpClient.GetAsync("api/farms");
            var responseDelete = await httpClient.DeleteAsync(string.Format("api/farms/{0}", randomFarmID));
            var responseAfterDelete = await httpClient.GetAsync("api/farms");

            var responseContent = await responseBeforeDelete.Content.ReadAsStringAsync();
            JObject parsedObject = JObject.Parse(responseContent);
            ShapedDataWithLinks dataBeforeDelete = parsedObject.ToObject<ShapedDataWithLinks>();

            responseContent = await responseAfterDelete.Content.ReadAsStringAsync();
            parsedObject = JObject.Parse(responseContent);
            ShapedDataWithLinks dataAfterDelete = parsedObject.ToObject<ShapedDataWithLinks>();

            // Assert
            dataBeforeDelete.Value.Count().Should().Be(3);
            responseDelete.StatusCode.Should().Be(HttpStatusCode.NoContent);
            dataAfterDelete.Value.Count().Should().Be(3);
        }
    }
}