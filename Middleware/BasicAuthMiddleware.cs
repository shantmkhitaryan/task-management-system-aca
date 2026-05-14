using System.Text;
using Microsoft.Extensions.DependencyInjection;
using task_management_system_aca.Services;

namespace task_management_system_aca.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        
        var path = context.Request.Path.ToString().ToLower();
        if (path.Contains("/swagger") ||
            path.Contains("/api/auth/register") ||
            path.Contains("/api/auth/login"))
        {
            await _next(context);
            return;
        }

        
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("WWW-Authenticate", "Basic");
            await context.Response.WriteAsync("Missing Authorization header");
            return;
        }

        try
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Authorization header format");
                return;
            }

            var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            var decodedBytes = Convert.FromBase64String(encodedCredentials);
            var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);
            var credentials = decodedCredentials.Split(':', 2);
            
            if (credentials.Length != 2)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid credentials format");
                return;
            }

            var username = credentials[0];
            var password = credentials[1];

            var authService = serviceProvider.GetRequiredService<AuthService>();
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
            await context.Response.WriteAsync("Authentication failed");
        }
    }
}