using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IEnumerable<UserWidgetDto>>> GetUserWidgets(Guid userId)
        {
            try
            {
                var widgetsAsEntities = await this.dataService.UserWidgets.FindByUserIdAsync(userId);

                var widgetsToReturn = this.mapper.Map<IEnumerable<UserWidgetDto>>(widgetsAsEntities);
                return GenericResponseBuilder.Success<IEnumerable<UserWidgetDto>>(widgetsToReturn);

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserWidgets. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<UserWidgetDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}