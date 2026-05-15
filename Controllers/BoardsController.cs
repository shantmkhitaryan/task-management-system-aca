using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Dto;
using task_management_system_aca.Entities;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly BoardService _boardService;

    public BoardsController(BoardService boardService)
    {
        _boardService = boardService;
    }

    private Guid GetCurrentUserId()
    {
        if (HttpContext.Items.TryGetValue("UserId", out var userIdObj))
        {
            return (Guid)userIdObj!;
        }
        throw new UnauthorizedAccessException("User not authenticated");
    }

    [HttpGet]
    public async Task<IActionResult> GetBoards()
    {
        var userId = GetCurrentUserId();
        var boards = await _boardService.GetBoardsByUserAsync(userId);
        return Ok(boards);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoard(Guid id)
    {
        var userId = GetCurrentUserId();
        var board = await _boardService.GetBoardByIdAsync(id, userId);
        
        if (board == null)
            return NotFound(new { error = "Board not found or you don't have access" });
        
        return Ok(board);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        var userId = GetCurrentUserId();
        
        var board = new Board
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Sku = request.Sku.ToUpper(),
            OwnerId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createdBoard = await _boardService.CreateBoardAsync(board);
        return CreatedAtAction(nameof(GetBoard), new { id = createdBoard.Id }, createdBoard);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(Guid id, [FromBody] UpdateBoardRequest request)
    {
        var userId = GetCurrentUserId();
        
        var board = new Board
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            OwnerId = userId
        };

        var updated = await _boardService.UpdateBoardAsync(board, userId);
        if (!updated)
            return NotFound(new { error = "Board not found or you don't have permission" });

        return Ok(new { message = "Board updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(Guid id)
    {
        var userId = GetCurrentUserId();
        
        var deleted = await _boardService.DeleteBoardAsync(id, userId);
        if (!deleted)
            return NotFound(new { error = "Board not found or you don't own it" });

        return NoContent();
    }
}