using System.Net.Http.Headers;
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

    public async Task InvokeAsync(HttpContext context, IServiceScopeFactory serviceScopeFactory)
    {
        
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/api/Auth/register"))
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
            if (string.IsNullOrEmpty(authHeader))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Authorization header");
                return;
            }

            var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
            if (authHeaderValue.Parameter == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid Authorization header");
                return;
            }

            var credentialBytes = Convert.FromBase64String(authHeaderValue.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

           
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
                var user = await authService.AuthenticateAsync(username, password);
                
                if (user == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid username or password");
                    return;
                }

                context.Items["User"] = user;
                context.Items["UserId"] = user.Id;
            }
            
            await _next(context);
        }
        catch
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Authorization header");
        }
    }
}