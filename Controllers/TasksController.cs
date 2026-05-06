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
    public async Task<IActionResult> GetTasks(int boardId, [FromQuery] bool archived = false)
    {
        var board = await _boardService.GetBoardByIdAsync(boardId);
        if (board == null)
            return NotFound($"Board {boardId} not found");

        var tasks = await _taskService.GetTasksByBoardIdAsync(boardId, archived);
        return Ok(tasks);
    }

    
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTask(int boardId, int taskId)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        return Ok(task);
    }

  
    [HttpPost]
    public async Task<IActionResult> CreateTask(int boardId, [FromBody] CreateTaskRequest request)
    {
        var board = await _boardService.GetBoardByIdAsync(boardId);
        if (board == null)
            return NotFound($"Board {boardId} not found");

        
        var sections = await _sectionService.GetSectionsByBoardIdAsync(boardId);
        
        if (sections == null || !sections.Any())
            return BadRequest("No sections found for this board. Please create a section first.");
        
        
        var defaultSection = sections.FirstOrDefault(s => s.IsDefault == true);
        
        if (defaultSection == null)
            defaultSection = sections.OrderBy(s => s.Position).FirstOrDefault();
        
        if (defaultSection == null)
            return BadRequest("No sections available for this board.");

        var newTaskId = new Random().Next(1, 1000000);
        
        var task = new TaskItem
        {
            Id = newTaskId,
            Title = request.Title,
            Description = request.Description ?? "",
            BoardId = boardId,
            SectionId = defaultSection.Id,
            AssigneeId = request.AssigneeId,
            DueDate = request.DueDate,
            Priority = request.Priority ?? "Medium",
            IsArchived = false,
            CreatedBy = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var taskId = await _taskService.CreateTaskAsync(task);
        
        return CreatedAtAction(nameof(GetTask), new { boardId, taskId }, new { id = taskId });
    }

    
    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(int boardId, int taskId, [FromBody] UpdateTaskRequest request)
    {
        var task = await _taskService.GetTaskByIdAsync(taskId);
        if (task == null || task.BoardId != boardId)
            return NotFound();

        task.Title = string.IsNullOrEmpty(request.Title) ? task.Title : request.Title;
        task.Description = request.Description ?? task.Description;
        task.AssigneeId = request.AssigneeId ?? task.AssigneeId;
        task.DueDate = request.DueDate ?? task.DueDate;
        task.Priority = string.IsNullOrEmpty(request.Priority) ? task.Priority : request.Priority;
        task.UpdatedAt = DateTime.UtcNow;

        var updated = await _taskService.UpdateTaskAsync(task);
        if (!updated)
            return BadRequest();

        return Ok(task);
    }

   
    [HttpPatch("{taskId}/move")]
    public async Task<IActionResult> MoveTask(int boardId, int taskId, [FromBody] MoveTaskRequest request)
    {
        
        var task = await _taskService.GetTaskByIdAsync(taskId);
        
        if (task == null)
        {
            return NotFound($"Task {taskId} not found in database");
        }
        
       
        if (task.BoardId != boardId)
        {
            await _taskService.FixTaskBoardIdAsync(taskId, boardId);
            task.BoardId = boardId;
        }

       
        
        var section = await _sectionService.GetSectionByIdAsync(request.SectionId);
        if (section == null)
        {
            return NotFound($"Section {request.SectionId} not found");
        }
        
        
        var moved = await _taskService.MoveTaskToSectionAsync(taskId, request.SectionId);
        if (!moved)
            return BadRequest("Failed to move task");

        return Ok(new { message = $"Task {taskId} moved to section {request.SectionId} successfully" });
    }

   
    [HttpPatch("{taskId}/archive")]
    public async Task<IActionResult> ArchiveTask(int boardId, int taskId)
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
    public async Task<IActionResult> DeleteTask(int boardId, int taskId)
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