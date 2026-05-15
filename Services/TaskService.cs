using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Data;
using task_management_system_aca.Dto;
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

    public async Task<List<TaskResponseDto>> GetTasksByBoardIdAsync(Guid boardId, Guid userId, bool includeArchived = false)
    {
        // Check if user has access to the board
        var hasAccess = await _context.Boards
            .AnyAsync(b => b.Id == boardId && 
                          (b.OwnerId == userId || 
                           _context.BoardMembers.Any(bm => bm.BoardId == b.Id && bm.UserId == userId)));
        
        if (!hasAccess) return new List<TaskResponseDto>();
        
        var query = _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.BoardId == boardId);
        
        if (!includeArchived)
        {
            query = query.Where(t => !t.IsArchived);
        }
        
        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        
        return tasks.Select(task => new TaskResponseDto
        {
            Id = task.Id,
            TaskReference = $"{task.Board.Sku}-{task.Id.ToString().Split('-').Last().Substring(0, 4).ToUpper()}",
            Title = task.Title,
            Description = task.Description,
            BoardId = task.BoardId,
            SectionId = task.SectionId,
            AssigneeId = task.AssigneeId,
            DueDate = task.DueDate,
            DueDateState = task.DueDateState,
            Priority = task.Priority,
            IsArchived = task.IsArchived,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        }).ToList();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(Guid taskId, Guid userId)
    {
        return await _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.Id == taskId && 
                        (t.Board.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == t.BoardId && bm.UserId == userId)))
            .FirstOrDefaultAsync();
    }

    public async Task<TaskResponseDto?> GetTaskResponseByIdAsync(Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.Id == taskId && 
                        (t.Board.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == t.BoardId && bm.UserId == userId)))
            .FirstOrDefaultAsync();
        
        if (task == null) return null;
        
        var shortId = task.Id.ToString().Split('-').Last().Substring(0, 4).ToUpper();
        var taskReference = $"{task.Board.Sku}-{shortId}";
        
        return new TaskResponseDto
        {
            Id = task.Id,
            TaskReference = taskReference,
            Title = task.Title,
            Description = task.Description,
            BoardId = task.BoardId,
            SectionId = task.SectionId,
            AssigneeId = task.AssigneeId,
            DueDate = task.DueDate,
            DueDateState = task.DueDateState,
            Priority = task.Priority,
            IsArchived = task.IsArchived,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    public async Task<bool> UpdateTaskAsync(TaskItem task, Guid userId)
    {
        var existingTask = await _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.Id == task.Id && 
                        (t.Board.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == t.BoardId && bm.UserId == userId)))
            .FirstOrDefaultAsync();
            
        if (existingTask == null) return false;
        
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.AssigneeId = task.AssigneeId;
        existingTask.DueDate = task.DueDate;
        existingTask.Priority = task.Priority;
        existingTask.UpdatedAt = DateTime.UtcNow;
        
        _context.Tasks.Update(existingTask);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> MoveTaskToSectionAsync(Guid taskId, Guid sectionId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.Id == taskId && 
                        (t.Board.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == t.BoardId && bm.UserId == userId)))
            .FirstOrDefaultAsync();
            
        if (task == null) return false;
        
        task.SectionId = sectionId;
        task.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ArchiveTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.Id == taskId && 
                        (t.Board.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == t.BoardId && bm.UserId == userId)))
            .FirstOrDefaultAsync();
            
        if (task == null) return false;
        
        task.IsArchived = true;
        task.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId, Guid userId)
    {
        var task = await _context.Tasks
            .Include(t => t.Board)
            .Where(t => t.Id == taskId && 
                        (t.Board.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == t.BoardId && bm.UserId == userId)))
            .FirstOrDefaultAsync();
            
        if (task == null) return false;
        
        _context.Tasks.Remove(task);
        return await _context.SaveChangesAsync() > 0;
    }
}