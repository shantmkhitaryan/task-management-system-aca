using Npgsql;
using Dapper;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class SectionService
{
    private readonly string _connectionString;

    public SectionService()
    {
        _connectionString = "Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123";
    }

    public async Task<int> CreateSectionAsync(Section section)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            INSERT INTO sections (id, name, board_id, position, is_default, created_at) 
            VALUES (@Id, @Name, @BoardId, @Position, @IsDefault, @CreatedAt)
            RETURNING id;
        ";
        
        return await connection.QuerySingleAsync<int>(sql, section);
    }

    public async Task<IEnumerable<Section>> GetSectionsByBoardIdAsync(int boardId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM sections WHERE board_id = @BoardId ORDER BY position ASC";
        
        return await connection.QueryAsync<Section>(sql, new { BoardId = boardId });
    }

    public async Task<Section?> GetSectionByIdAsync(int sectionId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM sections WHERE id = @SectionId";
        
        return await connection.QueryFirstOrDefaultAsync<Section>(sql, new { SectionId = sectionId });
    }
}