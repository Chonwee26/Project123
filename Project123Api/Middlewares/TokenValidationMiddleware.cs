using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
namespace Project123Api.Middlewares
{

    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the request path starts with "/api" (or any path you want to protect)
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                // Check for the Authorization header
                if (!context.Request.Headers.TryGetValue("UserToken", out var token) || string.IsNullOrEmpty(token))
                {
                    // If the token is missing, return a 401 Unauthorized response
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token is missing. Please log in.");
                    return;
                }

                // Optionally, validate the token format (e.g., "Bearer <token>")
                if (!token.ToString().StartsWith("Bearer "))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token format.");
                    return;
                }

                // Proceed with the next middleware in the pipeline
            }

            await _next(context);
        }
    }
}
