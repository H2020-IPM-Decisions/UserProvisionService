using System;
using System.Net.Http;
using System.Threading.Tasks;
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
        public Task<string> GetAccessTokenAsync()
        {
            return Task.FromResult(configuration["DSSInternalInformation:AuthTokens:gr.gaiasense.ipm_plasvi"]);
        }
    }
}
