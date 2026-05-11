using System.Text;
using task_management_system_aca.Services;

namespace task_management_system_aca.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        
        var path = context.Request.Path.ToString();
        if (path.Contains("/swagger") || path.Contains("/api/Auth/register"))
        {
            await _next(context);
            return;
        }

        
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Missing Authorization header");
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        
        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Authorization scheme");
            return;
        }

        try
        {
            var encoded = authHeader.Substring(6).Trim();
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var parts = decoded.Split(':', 2);
            
            if (parts.Length != 2)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid credentials format");
                return;
            }

            var username = parts[0];
            var password = parts[1];

            var user = await authService.AuthenticateAsync(username, password);
            
            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid username or password");
                return;
            }

            context.Items["User"] = user;
            context.Items["UserId"] = user.Id;
            
            await _next(context);
        }
        catch
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authentication error");
        }
    }
}