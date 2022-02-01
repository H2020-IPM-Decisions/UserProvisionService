using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    public class AddLanguageToContextFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var language = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault();
                if (string.IsNullOrEmpty(language)) language = "en";                
                context.HttpContext.Items.Add("language", language);
            }
            catch (Exception ex)
            {
                context.Result = new BadRequestObjectResult(new { message = ex.Message.ToString() }); ;
                return;
            }
        }
    }
}