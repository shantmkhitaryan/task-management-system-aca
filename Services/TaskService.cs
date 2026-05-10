using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Data;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class TaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<List<TaskItem>> GetTasksByBoardIdAsync(Guid boardId, bool includeArchived = false)
    {
        var query = _context.Tasks.Where(t => t.BoardId == boardId);
        
        if (!includeArchived)
        {
            query = query.Where(t => !t.IsArchived);
        }
        
        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> MoveTaskToSectionAsync(Guid taskId, Guid sectionId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;
        
        task.SectionId = sectionId;
        task.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ArchiveTaskAsync(Guid taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;
        
        task.IsArchived = true;
        task.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;
        
        _context.Tasks.Remove(task);
        return await _context.SaveChangesAsync() > 0;
    }
}