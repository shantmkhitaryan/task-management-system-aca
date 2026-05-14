using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Data;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class InviteService
{
    private readonly AppDbContext _context;

    public InviteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BoardInvite> CreateInvitationAsync(BoardInvite invite)
    {
        
        var existingInvite = await _context.BoardInvites
            .FirstOrDefaultAsync(i => i.BoardId == invite.BoardId && 
                                      i.InvitedUserId == invite.InvitedUserId && 
                                      i.Status == "Pending");
        
        if (existingInvite != null)
        {
            throw new InvalidOperationException("Pending invite already exists for this user and board");
        }
        
        _context.BoardInvites.Add(invite);
        await _context.SaveChangesAsync();
        return invite;
    }

    public async Task<List<BoardInvite>> GetBoardInvitesByBoardIdAsync(Guid boardId)
    {
        return await _context.BoardInvites
            .Where(i => i.BoardId == boardId)
            .Include(i => i.InvitedUser)
            .Include(i => i.Inviter)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<BoardInvite>> GetInvitesByUserAsync(Guid userId)
    {
        return await _context.BoardInvites
            .Where(i => i.InvitedUserId == userId && i.Status == "Pending")
            .Include(i => i.Board)
            .Include(i => i.Inviter)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<BoardInvite?> GetInvitationByIdAsync(Guid inviteId)
    {
        return await _context.BoardInvites
            .Include(i => i.Board)
            .Include(i => i.InvitedUser)
            .Include(i => i.Inviter)
            .FirstOrDefaultAsync(i => i.Id == inviteId);
    }

    public async Task<bool> UpdateInvitationStatusAsync(Guid inviteId, string status)
    {
        var invite = await _context.BoardInvites.FindAsync(inviteId);
        if (invite == null)
            return false;

        invite.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<bool> AddUserToBoardAsync(Guid boardId, Guid userId)
    {
       
        var existingMember = await _context.BoardMembers
            .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId);
        
        if (existingMember != null)
            return true; 

        var boardMember = new BoardMember
        {
            BoardId = boardId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };

        _context.BoardMembers.Add(boardMember);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteInvitationAsync(Guid inviteId)
    {
        var invite = await _context.BoardInvites.FindAsync(inviteId);
        if (invite == null)
            return false;

        _context.BoardInvites.Remove(invite);
        await _context.SaveChangesAsync();
        return true;
    }
}