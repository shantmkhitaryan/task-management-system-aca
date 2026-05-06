using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
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

    [HttpGet]
    public async Task<IActionResult> GetMyBoards([FromQuery] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest(new { error = "UserId is required" });
        }
        
        var boards = await _boardService.GetBoardsByUserAsync(userId);
        return Ok(boards);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBoard(int id)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        if (board == null)
            return NotFound(new { error = $"Board {id} not found" });
        
        return Ok(board);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBoard([FromBody] CreateBoardRequest request)
    {
        try
        {
            // Log what we received
            Console.WriteLine($"Received UserId: {request.UserId}");
            Console.WriteLine($"Received Title: {request.Title}");
            Console.WriteLine($"Received SKU: {request.Sku}");
            
            if (request.UserId <= 0)
            {
                return BadRequest(new { error = "UserId is required and must be greater than 0" });
            }
            
            var boardId = new Random().Next(1, 1000000);
            
            // Use direct SQL with NpgsqlConnection
            using var connection = new NpgsqlConnection("Host=localhost;Port=5432;Database=taskmanagement;Username=admin;Password=admin123");
            await connection.OpenAsync();
            
            // First verify the user exists
            var userCheck = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM users WHERE id = @id", new { id = request.UserId });
            Console.WriteLine($"User count: {userCheck}");
            
            if (userCheck == 0)
            {
                return BadRequest(new { error = $"User with ID {request.UserId} not found" });
            }
            
            var sql = @"
                INSERT INTO boards (id, title, description, sku, owner_id, created_at) 
                VALUES (@id, @title, @description, @sku, @ownerId, @createdAt)
            ";
            
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                id = boardId,
                title = request.Title,
                description = request.Description ?? "",
                sku = request.Sku.ToUpper(),
                ownerId = request.UserId,
                createdAt = DateTime.UtcNow
            });
            
            Console.WriteLine($"Rows affected: {rowsAffected}");
            
            return Ok(new { id = boardId, message = "Board created successfully" });
        }
        catch (PostgresException ex)
        {
            return StatusCode(500, new { error = ex.Message, detail = ex.Detail, sqlState = ex.SqlState });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBoard(int id, [FromBody] UpdateBoardRequest request)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        if (board == null)
            return NotFound(new { error = $"Board {id} not found" });

        board.Title = request.Title;
        board.Description = request.Description;

        var updated = await _boardService.UpdateBoardAsync(board);
        if (!updated)
            return BadRequest(new { error = "Update failed" });

        return Ok(board);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBoard(int id, [FromQuery] int ownerId)
    {
        if (ownerId <= 0)
        {
            return BadRequest(new { error = "OwnerId is required" });
        }
        
        var deleted = await _boardService.DeleteBoardAsync(id, ownerId);
        if (!deleted)
            return NotFound(new { error = $"Board {id} not found or you don't own it" });
        
        return NoContent();
    }
}