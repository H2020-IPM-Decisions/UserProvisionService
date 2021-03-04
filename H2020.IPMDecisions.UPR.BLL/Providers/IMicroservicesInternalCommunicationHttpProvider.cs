using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public interface IMicroservicesInternalCommunicationHttpProvider
    {
        Task<string> GetUserIdFromIdpMicroservice(string userEmail);
        Task<bool> SendDataRequestEmail(string requesterEmail, string toEmail);
        Task<DssExecutionInformation> GetDssInformationFromDssMicroservice(string dssId, string modelId);
    }
}