using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public interface IMicroservicesInternalCommunicationHttpProvider
    {
        Task<string> GetUserIdFromIdpMicroservice(string userEmail);
        Task<bool> SendDataRequestEmail(string requesterEmail, string toEmail);
    }
}