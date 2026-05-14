using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Data;
using task_management_system_aca.Entities;
using task_management_system_aca.Extensions;

namespace task_management_system_aca.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var hash = password.HashPassword();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash);
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var hash = password.HashPassword();
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hash);
    }

    public async Task<User> RegisterAsync(User user)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username {user.Username} already exists");
        }
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}