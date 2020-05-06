using System.Net.Http;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests
{
    public class FakeWebHost : IAsyncLifetime
    {
        private IHost Host;
        public HttpClient httpClient;
        public async Task InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.Test.json")
              .Build();

            Host = await new HostBuilder()
              .ConfigureWebHost(webBuilder =>
              {
                  webBuilder
                   .UseEnvironment(Microsoft.Extensions.Hosting.Environments.Staging)
                   .UseStartup<Startup>()
                   .UseTestServer()
                   .UseConfiguration(configuration);
              })
              .StartAsync();

            httpClient = Host.GetTestServer().CreateClient();
        }

        public async Task DisposeAsync()
        {
            httpClient?.Dispose();
            await Host?.StopAsync();
            Host?.Dispose();
        }

        [CollectionDefinition("FakeWebHost")]
        public class FakeWebHostCollectionFixture : ICollectionFixture<FakeWebHost>
        {
        }
    }
}