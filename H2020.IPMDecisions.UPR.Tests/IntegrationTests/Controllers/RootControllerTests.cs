using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    [Collection("FakeWebHost")]
    public class RootControllerTests
    {
        private readonly FakeWebHost fakeWebHost;

        public RootControllerTests(FakeWebHost fakeWebHost)
        {
            this.fakeWebHost = fakeWebHost;
        }

        [Fact]
        public async void Get_NoToken_ShouldReturnOK()
        {
            // Arrange

            // Act
            var response = await fakeWebHost.httpClient.GetAsync("api/");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Post_NoToken_ShouldReturnOK()
        {
            // Arrange
            var stringContent = new StringContent("");

            // Act
            var response = await fakeWebHost.httpClient.PostAsync("api/", stringContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }
    }
}