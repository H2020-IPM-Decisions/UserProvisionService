using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Configurations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public class DssAuthTokenService : IDssAuthTokenService
    {
        private readonly HttpClient httpClient;
        private readonly IMemoryCache cache;
        private readonly IConfiguration configuration;
        public DssAuthTokenService(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<string> GetAccessTokenAsync(string configString)
        {
            if (cache.TryGetValue("DssAccessToken", out string? accessToken) && accessToken != null)
            {
                return accessToken;
            }

            var response = await RequestTokenAsync(configString);

            if (!string.IsNullOrEmpty(response.AccessToken))
            {
                cache.Set("DssAccessToken", response.AccessToken, TimeSpan.FromSeconds(response.ExpiresIn - response.ExpiresIn / 10));
            }

            return response.AccessToken;
        }

        private async Task<IsipTokenResponse> RequestTokenAsync(string configString)
        {
            var deIsipTokenInformation = configuration.GetSection(configString);
            DeIsipTokenInformation deIsipTokenInformationConfiguration = new();
            deIsipTokenInformation.Bind(deIsipTokenInformationConfiguration);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["username"] = deIsipTokenInformationConfiguration.Username,
                ["password"] = deIsipTokenInformationConfiguration.Password,
                ["grant_type"] = "password",
                ["client_id"] = deIsipTokenInformationConfiguration.ClientId,
                ["client_secret"] = deIsipTokenInformationConfiguration.ClientSecret,
            });

            var request = new HttpRequestMessage(
            HttpMethod.Post,
                deIsipTokenInformationConfiguration.TokenEndpoint)
            {
                Content = content
            };

            var response = await httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<IsipTokenResponse>();

            if (tokenResponse == null || String.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new Exception("Failed to obtain access token from tile API.");
            }

            return tokenResponse;
        }
    }
}

public class IsipTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

}