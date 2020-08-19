using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    // This Filter Should be executed after "AddUserIdToContextFilter"
    public class FieldBelongsToUserActionFilter : IActionFilter
    {
        private readonly IDataService dataService;
        public FieldBelongsToUserActionFilter(IDataService dataService)
        {
            this.dataService = dataService 
                ?? throw new ArgumentNullException(nameof(dataService));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {      
            try
            {
                var userId = Guid.Parse(context.HttpContext.Items["userId"].ToString());
                var isAdmin = context.HttpContext.Items["isAdmin"];

                var fieldId = "";
                if (!context.ActionArguments.ContainsKey("fieldId"))
                {
                    context.Result = new BadRequestObjectResult("Bad farmId parameter");
                    return;
                }
                fieldId = context.ActionArguments["fieldId"].ToString();

                if (!Guid.TryParse(fieldId, out var validatedGuid))
                {
                    context.Result = new BadRequestObjectResult(new { message = "The 'FieldId' on the URL is invalid" });
                    return;
                }

                Field existingField =
                    FindFieldAsync(userId, isAdmin, validatedGuid)
                    .GetAwaiter()
                    .GetResult();

                if (existingField == null)
                {
                    context.Result = new NotFoundResult();
                    return;
                }
                context.HttpContext.Items.Add("field", existingField);
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { message = ex.Message });
                return;
            }            
        }

        private async Task<Field> FindFieldAsync(Guid userId, object isAdmin, Guid validatedGuid)
        {            
            Field existingField;

            if (isAdmin == null)
            {
                existingField = await this.dataService
                .Fields
                .FindByConditionAsync(
                    f => f.Id == validatedGuid
                    &&
                    f.Farm.UserFarms.Any(uf => uf.UserId == userId)
                );
            }
            else
            {
                existingField = await this.dataService
                    .Fields
                    .FindByIdAsync(validatedGuid);
            }
            return existingField;
        }
    }
}