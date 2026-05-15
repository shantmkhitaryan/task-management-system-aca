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
        var path = context.Request.Path.ToString().ToLower();
        
       
        if (path.Contains("swagger") || path.Contains("register") || path.Contains("login"))
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

        try
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            var encoded = authHeader.Replace("Basic ", "").Trim();
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var parts = decoded.Split(':', 2);
            
            var username = parts[0];
            var password = parts[1];

            var user = await authService.AuthenticateAsync(username, password);
            
            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid username or password");
                return;
            }

           
            context.Items["UserId"] = user.Id;
            context.Items["User"] = user;
            
            await _next(context);
        }
        catch (FormatException)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Base64 encoding");
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync($"Auth error: {ex.Message}");
        }
    }
}