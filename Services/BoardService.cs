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
        _context.Boards.Add(board);
        await _context.SaveChangesAsync();
        return board;
    }

    public async Task<List<Board>> GetBoardsByUserAsync(Guid userId)
    {
        return await _context.Boards
            .Where(b => b.OwnerId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Board?> GetBoardByIdAsync(Guid boardId)
    {
        return await _context.Boards
            .Include(b => b.Sections)
            .Include(b => b.Tasks)
            .FirstOrDefaultAsync(b => b.Id == boardId);
    }

    public async Task<bool> UpdateBoardAsync(Board board)
    {
        _context.Boards.Update(board);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteBoardAsync(Guid boardId, Guid ownerId)
    {
        var board = await _context.Boards
            .FirstOrDefaultAsync(b => b.Id == boardId && b.OwnerId == ownerId);
        
        if (board == null) return false;
        
        _context.Boards.Remove(board);
        return await _context.SaveChangesAsync() > 0;
    }
}