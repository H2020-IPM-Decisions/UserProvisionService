using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.JsonPatch;
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

        public async Task<GenericResponse> UpdateUserWidgets(Guid userId, JsonPatchDocument<UserWidgetForUpdateDto> patchDocument)
        {
            try
            {
                var widgetsAsEntities = await this.dataService.UserWidgets.FindByUserIdAsync(userId);
                foreach (var operation in patchDocument.Operations)
                {
                    switch (operation.op.ToLower())
                    {
                        case "replace":
                            var widgetDescription = operation.path.Split("/")[1];
                            var widget = widgetsAsEntities
                                .Where(w => w.Widget.Description.ToLower().Equals(widgetDescription.ToLower()))
                                .FirstOrDefault();
                            if (widget != null) widget.Allowed = bool.Parse(operation.value.ToString());
                            break;
                        default:
                            break;
                    }
                }
                this.dataService.UserWidgets.Update(widgetsAsEntities);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateUserWidgets. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}