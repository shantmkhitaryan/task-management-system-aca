using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Data;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class BoardService
{
    private readonly AppDbContext _context;

    public BoardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Board> CreateBoardAsync(Board board)
    {
        // 1. Add the board
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        
        // 2. ✅ AUTO-CREATE 3 SECTIONS
        var sections = new List<Section>
        {
            new Section
            {
                Id = Guid.NewGuid(),
                Name = "To Do",
                BoardId = board.Id,
                Position = 0,
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            },
            new Section
            {
                Id = Guid.NewGuid(),
                Name = "In Progress",
                BoardId = board.Id,
                Position = 1,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            },
            new Section
            {
                Id = Guid.NewGuid(),
                Name = "Done",
                BoardId = board.Id,
                Position = 2,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            }
        };
        
        _context.Sections.AddRange(sections);
        await _context.SaveChangesAsync();
        
        return board;
    }

    public async Task<List<Board>> GetBoardsByUserAsync(Guid userId)
    {
        return await _context.Boards
            .Where(b => b.OwnerId == userId ||
                        _context.BoardMembers.Any(bm => bm.BoardId == b.Id && bm.UserId == userId))
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Board?> GetBoardByIdAsync(Guid boardId, Guid userId)
    {
        return await _context.Boards
            .Where(b => b.Id == boardId && 
                        (b.OwnerId == userId || 
                         _context.BoardMembers.Any(bm => bm.BoardId == b.Id && bm.UserId == userId)))
            .Include(b => b.Sections)
            .Include(b => b.Tasks)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateBoardAsync(Board board, Guid userId)
    {
        var existingBoard = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == board.Id && 
                                      (b.OwnerId == userId || 
                                       _context.BoardMembers.Any(bm => bm.BoardId == b.Id && bm.UserId == userId)));
        
        if (existingBoard == null) return false;
        
        existingBoard.Title = board.Title;
        existingBoard.Description = board.Description;
        
        _context.Boards.Update(existingBoard);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteBoardAsync(Guid boardId, Guid userId)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == boardId && b.OwnerId == userId);
        
        if (board == null) return false;
        
        _context.Boards.Remove(board);
        return await _context.SaveChangesAsync() > 0;
    }
}