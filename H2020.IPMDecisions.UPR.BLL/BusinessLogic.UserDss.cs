using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<bool>> GetAllUserFieldCropPestDss(Guid userId)
        {
            try
            {
                return GenericResponseBuilder.Success<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllUserFieldCropPestDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<bool>(false, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}