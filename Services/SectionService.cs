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

    public async Task<List<Section>> GetSectionsByBoardIdAsync(Guid boardId)
    {
        return await _context.Sections
            .Where(s => s.BoardId == boardId)
            .OrderBy(s => s.Position)
            .ToListAsync();
    }

    public async Task<Section?> GetSectionByIdAsync(Guid sectionId)
    {
        return await _context.Sections
            .FirstOrDefaultAsync(s => s.Id == sectionId);
    }

    public async Task<bool> UpdateSectionNameAsync(Guid sectionId, string name)
    {
        var section = await _context.Sections.FindAsync(sectionId);
        if (section == null) return false;
        
        section.Name = name;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteSectionAsync(Guid sectionId)
    {
        var section = await _context.Sections.FindAsync(sectionId);
        if (section == null) return false;
        
        _context.Sections.Remove(section);
        return await _context.SaveChangesAsync() > 0;
    }
}