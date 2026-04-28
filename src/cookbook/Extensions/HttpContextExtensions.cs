using System;
using System.Security.Claims;

namespace cookbook.Extensions;

public static class HttpContextExtensions
{
    public static string GetUserId(this HttpContext context)
    {
        if (
            context.User.Identity?.IsAuthenticated == null
            || !context.User.Identity.IsAuthenticated
        )
        {
            return null;
        }
        string userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return userId;
    }
}
