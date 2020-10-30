using System;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace H2020.IPMDecisions.UPR.Tests
{
    public class FakeApiGatewayHost : IDisposable
    {
        private WireMockServer stub;
        public FakeApiGatewayHost()
        {
            stub = WireMockServer.Start(new WireMockServerSettings
            {
                Urls = new[] { "http://+:5002" },
                StartAdminInterface = true
            });

            stub.Given(
                Request.Create()
                    .WithPath("/api/idp/internal/getuserid")
                    .WithHeader("ipm-internal-auth", "1234")
                    .WithBody(new WildcardMatcher("*datashareuser1*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("0ec586be-332d-439b-b614-08bc581cd4cb"));

            stub.Given(
                Request.Create()
                    .WithPath("/api/idp/internal/getuserid")
                    .WithHeader("ipm-internal-auth", "1234")
                    .WithBody(new WildcardMatcher("*datashareuser2*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("4d0fc5dc-ab3a-4c5c-9363-e82a37175b83"));

            stub.Given(
               Request.Create()
                   .WithPath("/api/idp/internal/getuserid")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*datashareuser3*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBody("cb498100-8e47-4247-9c4d-6db2e256efaa"));
        }

        public void Dispose()
        {
            stub.Stop();
        }
    }

    public class FakeApiGatewayHostTests : IClassFixture<FakeApiGatewayHost>
    {
        private readonly FakeApiGatewayHost fixture;

        public FakeApiGatewayHostTests(FakeApiGatewayHost fixture)
        {
            this.fixture = fixture;
        }
    }
}