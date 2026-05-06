using Npgsql;
using Dapper;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class AuthService
{
    private readonly string _connectionString;

    public AuthService()
    {
        _connectionString = "Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123";
    }

    public async Task RegisterAsync(User user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "INSERT INTO users (id, username, password_hash, created_at) VALUES (@Id, @Username, @PasswordHash, @CreatedAt)";
        
        await connection.ExecuteAsync(sql, user);
    }
}