using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public interface IDssAuthTokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}
