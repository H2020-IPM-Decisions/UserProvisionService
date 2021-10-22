using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class CustomConflictResult : IActionResult
    {
        public string Detail { get; private set; }
        public object Result { get; private set; }

        public CustomConflictResult(string errorDetail, object result = null)
        {
            Detail = errorDetail;
            Result = result;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var problemDetails = new CustomProblemDetails
            {
                Title = "Conflict",
                Status = 409,
                Detail = Detail,
                Type = context.HttpContext.TraceIdentifier,
                Result = Result
            };

            context.HttpContext.Response.StatusCode = problemDetails.Status.Value;
            context.HttpContext.Response.ContentType = "application/problem+json";
            await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            await Task.CompletedTask;
        }
    }

    internal class CustomProblemDetails : ProblemDetails
    {
        public object Result { get; set; }
    }
}