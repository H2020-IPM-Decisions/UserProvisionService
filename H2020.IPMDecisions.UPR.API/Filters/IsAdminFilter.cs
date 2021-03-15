using System;
using System.Security.Claims;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace H2020.IPMDecisions.UPR.API.Filters
{
    public class IsAdminFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return false;

            var userIdFromToken = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdFromToken, out var validatedGuid)) return false;

            var userRoleFromToken = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

            if (userRoleFromToken != null && userRoleFromToken.ToString().ToLower() == "admin")
                return true;
            else
                return false;
        }
    }
}