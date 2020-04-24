// using System;
// using System.Net;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text;
// using System.Json;
// using FluentAssertions;
// using Microsoft.AspNetCore.TestHost;
// using Xunit;

// namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
// {
//     public class UserProfilesControllerTests : IDisposable
//     {
//         private FakeWebHostWithDb _fakeWebHost;

//         public UserProfilesControllerTests()
//         {
//             _fakeWebHost = new FakeWebHostWithDb(true);
//         }

//         public async void Dispose()
//         {
//             await _fakeWebHost.DisposeAsync();
//         }

//         [Fact]
//         public async void Get_AddminCanAccessController_NotFound()
//         {
//             // Arrange
//             var tokenUserId = Guid.NewGuid();
//             var resourceUserId = Guid.NewGuid();

//             var myToken = TokenGeneratorTests.GenerateToken(tokenUserId, "admin");

//             await _fakeWebHost.InitializeAsync();
//             var httpClient = _fakeWebHost.Host.GetTestServer().CreateClient();

//             httpClient
//                 .DefaultRequestHeaders
//                 .Authorization =
//                 new AuthenticationHeaderValue("Bearer", myToken);

//             httpClient
//                 .DefaultRequestHeaders
//                 .Accept
//                 .Add(new MediaTypeWithQualityHeaderValue("application/json"));

//             // Act
//             var response = await httpClient.GetAsync(string.Format("api/users/{0}/profiles", resourceUserId));

//             // Assert
//             response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//         }
//     }
// }