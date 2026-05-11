using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Dto;
using task_management_system_aca.Entities;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers;

[ApiController]
[Route("api/boards/{boardId}/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;
    private readonly BoardService _boardService;
    private readonly SectionService _sectionService;

    public TasksController(TaskService taskService, BoardService boardService, SectionService sectionService)
    {
        _taskService = taskService;
        _boardService = boardService;
        _sectionService = sectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks(Guid boardId, [FromQuery] bool archived = false)
    {
        var board = await _boardService.GetBoardByIdAsync(boardId);
        if (board == null)
            return NotFound($"Board {boardId} not found");

        var tasks = await _taskService.GetTasksByBoardIdAsync(boardId, archived);
        return Ok(tasks);
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTask(Guid boardId, Guid taskId)
    {
        var task = await _taskService.GetTaskResponseByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(Guid boardId, [FromBody] CreateTaskRequest request)
    {
        var board = await _boardService.GetBoardByIdAsync(boardId);
        if (board == null)
            return NotFound($"Board {boardId} not found");

        var sections = await _sectionService.GetSectionsByBoardIdAsync(boardId);
        
        if (sections == null || !sections.Any())
            return BadRequest("No sections found. Please create a section first.");
        
        var defaultSection = sections.FirstOrDefault(s => s.IsDefault);
        
        if (defaultSection == null)
            defaultSection = sections.OrderBy(s => s.Position).FirstOrDefault();

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            BoardId = boardId,
            SectionId = defaultSection!.Id,
            AssigneeId = request.AssigneeId,
            DueDate = request.DueDate,
            Priority = request.Priority,
            IsArchived = false,
            CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTask = await _taskService.CreateTaskAsync(task);
        return CreatedAtAction(nameof(GetTask), new { boardId, taskId = createdTask.Id }, new { id = createdTask.Id });
    }

    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(Guid boardId, Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        if (!string.IsNullOrEmpty(request.Title))
            task.Title = request.Title;
        
        if (request.Description != null)
            task.Description = request.Description;
        
        if (request.AssigneeId.HasValue)
            task.AssigneeId = request.AssigneeId;
        
        if (request.DueDate.HasValue)
            task.DueDate = request.DueDate;
        
        if (!string.IsNullOrEmpty(request.Priority))
            task.Priority = request.Priority;
        
        task.UpdatedAt = DateTime.UtcNow;

        var updated = await _taskService.UpdateTaskAsync(task);
        if (!updated)
            return BadRequest();

        return Ok(task);
    }

    [HttpPatch("{taskId}/move")]
    public async Task<IActionResult> MoveTask(Guid boardId, Guid taskId, [FromBody] MoveTaskRequest request)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        var moved = await _taskService.MoveTaskToSectionAsync(taskId, request.SectionId);
        if (!moved)
            return BadRequest();

        return Ok(new { message = "Task moved successfully" });
    }

    [HttpPatch("{taskId}/archive")]
    public async Task<IActionResult> ArchiveTask(Guid boardId, Guid taskId)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        var archived = await _taskService.ArchiveTaskAsync(taskId);
        if (!archived)
            return BadRequest();

        return Ok(new { message = "Task archived successfully" });
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid boardId, Guid taskId)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        var deleted = await _taskService.DeleteTaskAsync(taskId);
        if (!deleted)
            return BadRequest();

        return NoContent();
    }
}