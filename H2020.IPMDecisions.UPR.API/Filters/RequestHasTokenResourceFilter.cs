using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    public class RequestHasInternalTokenResourceFilter : Attribute, IResourceFilter
    {
        private readonly IConfiguration configuration;

        public RequestHasInternalTokenResourceFilter(IConfiguration configuration)
        {
            this.configuration = configuration 
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string tokenHeader = context.HttpContext.Request.Headers[configuration["MicroserviceInternalCommunication:SecurityTokenCustomHeader"]];

            if (string.IsNullOrEmpty(tokenHeader))
            {
                context.Result = new UnauthorizedResult();
                return;           
            }
            if (tokenHeader.Trim() != configuration["MicroserviceInternalCommunication:SecurityToken"])
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            return;            
        }
    }
}