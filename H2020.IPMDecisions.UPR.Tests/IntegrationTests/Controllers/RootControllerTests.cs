using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests.IntegrationTests.Controllers
{
    public class RootControllerTests : IDisposable
    {
        private FakeWebHost _fakeWebHost;
        public RootControllerTests()
        {
            _fakeWebHost = new FakeWebHost();
        }

        public async void Dispose()
        {
            await _fakeWebHost.DisposeAsync();
        }

        [Fact]
        public async void Get_NoToken_ShouldReturnOK()
        {
            //Given
            await _fakeWebHost.InitializeAsync();
            var httpClient = _fakeWebHost.Host.GetTestServer().CreateClient();

            //When
            var response = await httpClient.GetAsync("api/");

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void Post_NoToken_ShouldReturnOK()
        {
            //Given
            await _fakeWebHost.InitializeAsync();
            var httpClient = _fakeWebHost.Host.GetTestServer().CreateClient();
            var stringContent = new StringContent("");

            //When
            var response = await httpClient.PostAsync("api/", stringContent);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }        
    }
}