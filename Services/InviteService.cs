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
        _context.BoardInvites.Add(invite);
        await _context.SaveChangesAsync();
        return invite;
    }

    public async Task<List<BoardInvite>> GetBoardInvitesByBoardIdAsync(Guid boardId)
    {
        return await _context.BoardInvites
            .Where(i => i.BoardId == boardId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> UpdateInvitationStatusAsync(Guid inviteId, string status)
    {
        var invite = await _context.BoardInvites.FindAsync(inviteId);
        if (invite == null) return false;
        
        invite.Status = status;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteInvitationAsync(Guid inviteId)
    {
        var invite = await _context.BoardInvites.FindAsync(inviteId);
        if (invite == null) return false;
        
        _context.BoardInvites.Remove(invite);
        return await _context.SaveChangesAsync() > 0;
    }
}