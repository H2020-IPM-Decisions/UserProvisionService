using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
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

        public async Task<GenericResponse> UpdateAdminVariableById(AdminValuesEnum id, AdminVariableForManipulationDto adminVariableForManipulationDto)
        {
            try
            {
                var adminVariable = await this.dataService.AdminVariables.FindByIdAsync(id);
                if (adminVariable == null)
                {
                    return GenericResponseBuilder.NotFound<AdministrationVariable>();
                }

                this.mapper.Map(adminVariableForManipulationDto, adminVariable);
                this.dataService.AdminVariables.Update(adminVariable);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateAdminVariable. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}