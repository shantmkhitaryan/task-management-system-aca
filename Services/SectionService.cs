using Microsoft.EntityFrameworkCore;
using task_management_system_aca.Data;
using task_management_system_aca.Entities;

namespace task_management_system_aca.Services;

public class SectionService
{
    private readonly AppDbContext _context;

    public SectionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Section> CreateSectionAsync(Section section)
    {
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
        return section;
    }

    public async Task<List<Section>> GetSectionsByBoardIdAsync(Guid boardId, Guid userId)
    {
        
        var hasAccess = await _context.Boards
            .AnyAsync(b => b.Id == boardId && 
                          (b.OwnerId == userId || 
                           _context.BoardMembers.Any(bm => bm.BoardId == b.Id && bm.UserId == userId)));
        
        if (!hasAccess) return new List<Section>();
        
        return await _context.Sections
            .Where(s => s.BoardId == boardId)
            .OrderBy(s => s.Position)
            .ToListAsync();
    }

    public async Task<Section?> GetSectionByIdAsync(Guid sectionId, Guid userId)
    {
        return await _context.Sections
            .Where(s => s.Id == sectionId && 
                        (_context.Boards.Any(b => b.Id == s.BoardId && 
                                                  (b.OwnerId == userId || 
                                                   _context.BoardMembers.Any(bm => bm.BoardId == b.Id && bm.UserId == userId)))))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateSectionNameAsync(Guid sectionId, string name, Guid userId)
    {
        var section = await _context.Sections
            .Where(s => s.Id == sectionId && 
                        (_context.Boards.Any(b => b.Id == s.BoardId && b.OwnerId == userId)))
            .FirstOrDefaultAsync();
            
        if (section == null) return false;
        
        section.Name = name;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteSectionAsync(Guid sectionId, Guid userId)
    {
        var section = await _context.Sections
            .Where(s => s.Id == sectionId && 
                        s.IsDefault == false &&
                        (_context.Boards.Any(b => b.Id == s.BoardId && b.OwnerId == userId)))
            .FirstOrDefaultAsync();
            
        if (section == null) return false;
        
        _context.Sections.Remove(section);
        return await _context.SaveChangesAsync() > 0;
    }
}