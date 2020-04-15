using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    public class UserAccessingOwnDataActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var claimsIdentity = context.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userRoleFromToken = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;      
            if (userRoleFromToken != null && userRoleFromToken.ToString().ToLower() == "admin")
                return;            

            var userIdFromToken = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;            
            if (!Guid.TryParse(userIdFromToken, out var validatedGuid)){
                context.Result = new BadRequestObjectResult(new { message = "The 'UserId' on token invalid" });
                return;
            }
            if (validatedGuid.ToString() != context.ActionArguments["userId"].ToString())
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}