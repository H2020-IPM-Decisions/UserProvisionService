using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    public class LocationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                var language = context.Request.Headers["Accept-Language"].FirstOrDefault();
                if (string.IsNullOrEmpty(language)) language = "en";
                if (language.Split(";").Count() > 1) language = "en";

                // ToDo - Remove when DSS API fixed to follow ISO
                if (language == "sl") language = "si";

                context.Items.Add("language", language);
                var culture = new CultureInfo(language);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                await next(context);
            }
            catch (Exception)
            {
                context.Abort();
                return;
            }
        }
    }
}