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
                Urls = new[] { "http://+:7008" },
                StartAdminInterface = true
            });

            stub.Given(
                Request.Create()
                    .WithPath("/api/idp/internal/getuserid")
                    .WithHeader("ipm-internal-auth", "1234")
                    .WithBody(new WildcardMatcher("*usernoprofile*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("0ec586be-332d-439b-b614-08bc581cd4cb"));

            stub.Given(
                Request.Create()
                    .WithPath("/api/idp/internal/getuserid")
                    .WithHeader("ipm-internal-auth", "1234")
                    .WithBody(new WildcardMatcher("*advisor*"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("4d0fc5dc-ab3a-4c5c-9363-e82a37175b83"));

            stub.Given(
               Request.Create()
                   .WithPath("/api/idp/internal/getuserid")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*advisornoprofile*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBody("cb498100-8e47-4247-9c4d-6db2e256efaa"));

            stub.Given(
               Request.Create()
                   .WithPath("/api/idp/internal/getuserid")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*defaultnormaluserid*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBody("89f4cb8a-c803-11ea-87d0-0242ac130003")); // Same as DB id

            stub.Given(
               Request.Create()
                   .WithPath("/api/idp/internal/getuserid")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*nosendemail*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBody("68840b5c-803b-461e-a262-cdb9932d203b")); // Same as DB id

            stub.Given(
               Request.Create()
                   .WithPath("/api/idp/internal/getuserid")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*existingDataShareRequest*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBody("d88ad6d9-c756-4901-ae22-8c7a1c178555")); // Same as DB id

            stub.Given(
               Request.Create()
                   .WithPath("/api/idp/internal/getuserid")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*dataShareRequestDeclined*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200)
                   .WithBody("91f59dba-cd51-4dc9-ada9-3d21e4f82351")); // Same as DB id

            stub.Given(
               Request.Create()
                   .WithPath("/api/eml/internal/SendDataRequest")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*defaultnormaluserid*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(200));

            stub.Given(
               Request.Create()
                   .WithPath("/api/eml/internal/SendDataRequest")
                   .WithHeader("ipm-internal-auth", "1234")
                   .WithBody(new WildcardMatcher("*nosendemail*"))
                   .UsingPost())
               .RespondWith(Response.Create()
                   .WithStatusCode(400));
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