

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class RoleAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        var authorizeAttribute = endpoint.Metadata
            .GetMetadata<AuthorizeAttribute>();

        if (authorizeAttribute == null)
        {
            await _next(context);
            return;
        }

        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(userRole))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
            return;
        }

        var requiredRoles = authorizeAttribute.Roles?.Split(',');
        if (requiredRoles != null && !requiredRoles.Contains(userRole))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { message = "Forbidden" });
            return;
        }

        await _next(context);
    }
}