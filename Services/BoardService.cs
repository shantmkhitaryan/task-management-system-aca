using Npgsql;
using Dapper;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class BoardService
{
    private readonly string _connectionString;

    public BoardService()
    {
        _connectionString = "Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123";
    }

    public async Task<int> CreateBoardAsync(Board board)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            INSERT INTO boards (id, title, description, sku, owner_id, created_at) 
            VALUES (@Id, @Title, @Description, @Sku, @OwnerId, @CreatedAt)
            RETURNING id;
        ";
        
        return await connection.QuerySingleAsync<int>(sql, board);
    }

    public async Task<IEnumerable<Board>> GetBoardsByUserAsync(int userId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM boards WHERE owner_id = @UserId ORDER BY created_at DESC";
        
        return await connection.QueryAsync<Board>(sql, new { UserId = userId });
    }

    public async Task<Board?> GetBoardByIdAsync(int boardId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM boards WHERE id = @BoardId";
        
        return await connection.QueryFirstOrDefaultAsync<Board>(sql, new { BoardId = boardId });
    }

    public async Task<bool> UpdateBoardAsync(Board board)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "UPDATE boards SET title = @Title, description = @Description WHERE id = @Id AND owner_id = @OwnerId";
        
        var affected = await connection.ExecuteAsync(sql, board);
        return affected > 0;
    }

    public async Task<bool> DeleteBoardAsync(int boardId, int ownerId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "DELETE FROM boards WHERE id = @BoardId AND owner_id = @OwnerId";
        
        var affected = await connection.ExecuteAsync(sql, new { BoardId = boardId, OwnerId = ownerId });
        return affected > 0;
    }
}