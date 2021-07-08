using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IEnumerable<AdminVariableDto>>> GetAllAdminVariables()
        {
            try
            {
                var adminVariables = await this.dataService.AdminVariables.FindAllAsync();
                var variablesToReturn = this.mapper.Map<IEnumerable<AdminVariableDto>>(adminVariables);
                return GenericResponseBuilder.Success<IEnumerable<AdminVariableDto>>(variablesToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAdminVariables. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<AdminVariableDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}