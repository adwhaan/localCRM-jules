using LocalCRM.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LocalCRM.API.Middleware;

public class ForcedPasswordChangeMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly string[] AllowedPaths = { "/api/auth/complete-password-change", "/api/auth/logout", "/graphql" };

    public ForcedPasswordChangeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
<<<<<<< HEAD

            // Allow access to password change, logout, and GraphQL (GraphQL needs internal handling or a specific mutation check)
            // For GraphQL, we might need a more granular check inside the resolver,
=======

            // Allow access to password change, logout, and GraphQL (GraphQL needs internal handling or a specific mutation check)
            // For GraphQL, we might need a more granular check inside the resolver,
>>>>>>> origin/main
            // but for a "middleware level redirect" on REST, we block other api calls.
            if (!AllowedPaths.Any(p => path.StartsWith(p)))
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user?.MustChangePassword == true)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            code = "password_change_required",
                            message = "You must change your password before you can proceed."
                        });
                        return;
                    }
                }
            }
        }

        await _next(context);
    }
}
