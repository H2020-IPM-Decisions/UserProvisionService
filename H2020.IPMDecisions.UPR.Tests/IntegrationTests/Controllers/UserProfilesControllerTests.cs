using System;
using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    public class UserProfilesControllerTests
    {
        private FakeWebHost _fakeWebHost;

        public UserProfilesControllerTests()
        {
            _fakeWebHost = new FakeWebHost(false);
        }

        [Fact]
        public async void Get_DifferentUserIdUrlAndToken_Unauthorized()
        {
            // Arrange
            var tokenUserId = Guid.NewGuid();
            var resourceUserId = Guid.NewGuid();

            var myToken = TokenGeneratorTests.GenerateToken(tokenUserId); 

            await _fakeWebHost.InitializeAsync();
            var httpClient = _fakeWebHost.Host.GetTestServer().CreateClient();

            httpClient
                .DefaultRequestHeaders
                .Authorization =
                new AuthenticationHeaderValue("Bearer", myToken);
            
            httpClient
                .DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //When
            var response = await httpClient.GetAsync(string.Format("api/users/{0}/profiles", resourceUserId));

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}