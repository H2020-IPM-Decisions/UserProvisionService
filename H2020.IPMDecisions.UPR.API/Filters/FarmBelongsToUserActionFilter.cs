using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    // This Filter Should be executed after "AddUserIdToContextFilter"
    public class FarmBelongsToUserActionFilter : IActionFilter
    {
        private readonly IDataService dataService;
        public FarmBelongsToUserActionFilter(IDataService dataService)
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
                var isAdmin = bool.Parse(context.HttpContext.Items["isAdmin"].ToString());

                var farmId = "";
                if (!context.ActionArguments.ContainsKey("farmId"))
                {
                    context.Result = new BadRequestObjectResult("Bad farmId parameter");
                    return;
                }
                farmId = context.ActionArguments["farmId"].ToString();

                if (!Guid.TryParse(farmId, out var validatedGuid))
                {
                    context.Result = new BadRequestObjectResult(new { message = "The 'FarmId' on the URL is invalid" });
                    return;
                }

                Farm existingFarm =
                    FindFarmAsync(userId, isAdmin, validatedGuid)
                    .GetAwaiter()
                    .GetResult();

                if (existingFarm == null)
                {
                    context.Result = new NotFoundResult();
                    return;
                }
                context.HttpContext.Items.Add("farm", existingFarm);
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { message = ex.Message });
                return;
            }
        }

        private async Task<Farm> FindFarmAsync(Guid userId, bool isAdmin, Guid validatedGuid)
        {
            Farm existingFarm;

            if (isAdmin == false)
            {
                existingFarm = await this.dataService
                .Farms
                .FindByConditionAsync(
                    f => f.Id == validatedGuid
                    &&
                    f.UserFarms.Any(uf => uf.UserId == userId),
                    true
                );
            }
            else
            {
                existingFarm = await this.dataService
                    .Farms
                    .FindByIdAsync(validatedGuid, true);
            }
            return existingFarm;
        }
    }
}