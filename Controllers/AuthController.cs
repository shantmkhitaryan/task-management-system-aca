using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Dto;
using task_management_system_aca.Entities;
using task_management_system_aca.Extensions;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = request.Password.HashPassword(),
                CreatedAt = DateTime.UtcNow
            };

            await _authService.RegisterAsync(user);
            
            return Ok(new { message = "Registration successful", userId = user.Id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = "Username and password are required" });
        }

        var user = await _authService.LoginAsync(request.Username, request.Password);
        
        if (user == null)
        {
            return Unauthorized(new { error = "Invalid username or password" });
        }
        
        return Ok(new 
        { 
            message = "Login successful",
            userId = user.Id,
            username = user.Username
        });
    }
}