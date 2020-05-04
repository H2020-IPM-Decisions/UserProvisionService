//using System;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Json;
//using FluentAssertions;
//using Microsoft.AspNetCore.TestHost;
//using Xunit;
//using Newtonsoft.Json;
//using H2020.IPMDecisions.UPR.Core.Dtos;
//using Microsoft.AspNetCore.JsonPatch;
//using Microsoft.AspNetCore.Mvc;

//namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
//{
//    public class UserProfilesControllerTests : IClassFixture<FakeWebHostWithDb>
//    {
//        private FakeWebHostWithDb fakeWebHost;
//        private string myAdminToken;
//        public UserProfilesControllerTests(FakeWebHostWithDb fakeWebHost)
//        {
//            this.fakeWebHost = fakeWebHost;

//            var tokenUserId = Guid.NewGuid();
//            myAdminToken = TokenGeneratorTests.GenerateToken(tokenUserId, "admin");
//        }

//        #region Admin Role
//        [Fact(Skip = "Needs database Connection")]
//        public async void Post_AdminCreatesOtherProfile_Created()
//        {
//            // Arrange            
//            var resourceUserId = Guid.NewGuid();
                       
//            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

//           httpClient
//                .DefaultRequestHeaders
//                .Authorization =
//                new AuthenticationHeaderValue("Bearer", myAdminToken);
            
//            httpClient
//               .DefaultRequestHeaders
//               .Accept
//               .Add(new MediaTypeWithQualityHeaderValue("application/json"));
           
//            var jsonObject = new JsonObject();
//            jsonObject.Add("firstName", "SomeName");
//            jsonObject.Add("lastName", "SomeLastNameName");
//            var content = new StringContent(
//                jsonObject.ToString(),
//                Encoding.UTF8,
//                "application/json");

//            // Act
//            var response = await httpClient.PostAsync(string.Format("api/users/{0}/profiles", resourceUserId), content);
//            var responseContent = await response.Content.ReadAsStringAsync();
//            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileDto>(responseContent);

//            // Assert
//            response.StatusCode.Should().Be(HttpStatusCode.Created);
//            responseDeserialized.UserId.Should().Be(resourceUserId);
//            responseDeserialized.FullName.Should().Be("SomeName SomeLastNameName");
            
//        }

//        [Fact(Skip = "Needs database Connection")]
//        public async void Get_AdminUsesUnexistingUserId_NotFound()
//        {
//            // Arrange
//            var resourceUserId = Guid.NewGuid();
//            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

//            httpClient
//                 .DefaultRequestHeaders
//                 .Authorization =
//                 new AuthenticationHeaderValue("Bearer", myAdminToken);

//            httpClient
//               .DefaultRequestHeaders
//               .Accept
//               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

//            // Act
//            var response = await httpClient.GetAsync(string.Format("api/users/{0}/profiles", resourceUserId));

//            // Assert
//            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//        }

//        [Fact(Skip = "Needs database Connection")]
//        public async void Delete_AdminUsesUnexistingUserId_NotContent()
//        {
//            // Arrange
//            var resourceUserId = Guid.NewGuid();
//            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

//            httpClient
//                 .DefaultRequestHeaders
//                 .Authorization =
//                 new AuthenticationHeaderValue("Bearer", myAdminToken);

//            httpClient
//               .DefaultRequestHeaders
//               .Accept
//               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

//            // Act
//            var response = await httpClient.DeleteAsync(string.Format("api/users/{0}/profiles", resourceUserId));

//            // Assert
//            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
//        }

//        [Fact(Skip = "Needs database Connection")]
//        public async void Patch_AdminUsesUnexistingUserId_CreatesNewUserProfile()
//        {
//            // Arrange
//            var resourceUserId = Guid.NewGuid();
//            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

//            httpClient
//                 .DefaultRequestHeaders
//                 .Authorization =
//                 new AuthenticationHeaderValue("Bearer", myAdminToken);

//            httpClient
//               .DefaultRequestHeaders
//               .Accept
//               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

//            var patchDoc = new JsonPatchDocument<UserProfileForUpdateDto>();
//            patchDoc.Add(u => u.FirstName, "NewName");
//            var serializedPatchDoc = JsonConvert.SerializeObject(patchDoc);
//            var content = new StringContent(
//                serializedPatchDoc,
//                Encoding.UTF8,
//                "application/json-patch+json");

//            // Act
//            var response = await httpClient.PatchAsync(string.Format("api/users/{0}/profiles", resourceUserId), content);
//            var responseContent = await response.Content.ReadAsStringAsync();
//            var responseDeserialized = JsonConvert.DeserializeObject<UserProfileDto>(responseContent);

//            // Assert
//            response.StatusCode.Should().Be(HttpStatusCode.Created);
//            responseDeserialized.UserId.Should().Be(resourceUserId);
//            responseDeserialized.FullName.Should().Be("NewName");
//        }

//        [Fact(Skip = "Needs database Connection")]
//        public async void Patch_AdminUsesUnexistingUserIdButNoFirstName_BadRequest()
//        {
//            // Arrange
//            var resourceUserId = Guid.NewGuid();
//            var httpClient = fakeWebHost.Host.GetTestServer().CreateClient();

//            httpClient
//                 .DefaultRequestHeaders
//                 .Authorization =
//                 new AuthenticationHeaderValue("Bearer", myAdminToken);

//            httpClient
//               .DefaultRequestHeaders
//               .Accept
//               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

//            var patchDoc = new JsonPatchDocument<UserProfileForUpdateDto>();
//            patchDoc.Add(u => u.LastName, "NewLastNameName");
//            var serializedPatchDoc = JsonConvert.SerializeObject(patchDoc);
//            var content = new StringContent(
//                serializedPatchDoc,
//                Encoding.UTF8,
//                "application/json-patch+json");

//            // Act
//            var response = await httpClient.PatchAsync(string.Format("api/users/{0}/profiles", resourceUserId), content);
//            var responseContent = await response.Content.ReadAsStringAsync();
//            var responseDeserialized = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

//            // Assert
//            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//            responseDeserialized.Title.Should().Be("One or more validation errors occurred.");
//        }
//        #endregion        
//    }
//}