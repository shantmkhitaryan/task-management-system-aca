using Npgsql;
using Dapper;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class TaskService
{
    private readonly string _connectionString;

    public TaskService()
    {
        _connectionString = "Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123";
    }

    // Create a new task
    public async Task<int> CreateTaskAsync(TaskItem task)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            INSERT INTO tasks (id, title, description, board_id, section_id, assignee_id, due_date, priority, is_archived, created_by, created_at, updated_at) 
            VALUES (@Id, @Title, @Description, @BoardId, @SectionId, @AssigneeId, @DueDate, @Priority, @IsArchived, @CreatedBy, @CreatedAt, @UpdatedAt)
            RETURNING id;
        ";
        
        return await connection.QuerySingleAsync<int>(sql, task);
    }

    // Get all tasks for a board
    public async Task<IEnumerable<TaskItem>> GetTasksByBoardIdAsync(int boardId, bool includeArchived = false)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM tasks WHERE board_id = @BoardId";
        
        if (!includeArchived)
        {
            sql += " AND is_archived = false";
        }
        
        sql += " ORDER BY created_at DESC";
        
        return await connection.QueryAsync<TaskItem>(sql, new { BoardId = boardId });
    }

    // Get a single task by ID
    public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM tasks WHERE id = @TaskId";
        
        return await connection.QueryFirstOrDefaultAsync<TaskItem>(sql, new { TaskId = taskId });
    }

    // Update a task
    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            UPDATE tasks 
            SET title = @Title, 
                description = @Description, 
                section_id = @SectionId, 
                assignee_id = @AssigneeId, 
                due_date = @DueDate, 
                priority = @Priority, 
                updated_at = @UpdatedAt
            WHERE id = @Id
        ";
        
        var affected = await connection.ExecuteAsync(sql, task);
        return affected > 0;
    }

    // Move task to a different section
    public async Task<bool> MoveTaskToSectionAsync(int taskId, int sectionId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            UPDATE tasks 
            SET section_id = @SectionId, updated_at = NOW() 
            WHERE id = @TaskId
        ";
        
        var affected = await connection.ExecuteAsync(sql, new { TaskId = taskId, SectionId = sectionId });
        return affected > 0;
    }

    // Archive a task
    public async Task<bool> ArchiveTaskAsync(int taskId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            UPDATE tasks 
            SET is_archived = true, updated_at = NOW() 
            WHERE id = @TaskId
        ";
        
        var affected = await connection.ExecuteAsync(sql, new { TaskId = taskId });
        return affected > 0;
    }

    // Unarchive a task
    public async Task<bool> UnarchiveTaskAsync(int taskId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            UPDATE tasks 
            SET is_archived = false, updated_at = NOW() 
            WHERE id = @TaskId
        ";
        
        var affected = await connection.ExecuteAsync(sql, new { TaskId = taskId });
        return affected > 0;
    }

    // Delete a task
    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "DELETE FROM tasks WHERE id = @TaskId";
        
        var affected = await connection.ExecuteAsync(sql, new { TaskId = taskId });
        return affected > 0;
    }

    // Fix incorrect board_id for a task
    public async Task<bool> FixTaskBoardIdAsync(int taskId, int correctBoardId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "UPDATE tasks SET board_id = @BoardId WHERE id = @TaskId";
        
        var affected = await connection.ExecuteAsync(sql, new { TaskId = taskId, BoardId = correctBoardId });
        return affected > 0;
    }

    // Get tasks by section
    public async Task<IEnumerable<TaskItem>> GetTasksBySectionIdAsync(int sectionId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM tasks WHERE section_id = @SectionId AND is_archived = false ORDER BY created_at DESC";
        
        return await connection.QueryAsync<TaskItem>(sql, new { SectionId = sectionId });
    }

    // Get tasks assigned to a user
    public async Task<IEnumerable<TaskItem>> GetTasksByAssigneeAsync(int assigneeId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = "SELECT * FROM tasks WHERE assignee_id = @AssigneeId AND is_archived = false ORDER BY due_date ASC";
        
        return await connection.QueryAsync<TaskItem>(sql, new { AssigneeId = assigneeId });
    }

    // Get overdue tasks
    public async Task<IEnumerable<TaskItem>> GetOverdueTasksAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var sql = @"
            SELECT * FROM tasks 
            WHERE due_date < NOW() 
            AND is_archived = false 
            AND is_archived = false
            ORDER BY due_date ASC
        ";
        
        return await connection.QueryAsync<TaskItem>(sql);
    }
}