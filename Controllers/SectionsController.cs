using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Dto;
using task_management_system_aca.Entities;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly SectionService _sectionService;

    public SectionsController(SectionService sectionService)
    {
        _sectionService = sectionService;
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
    public async Task<IActionResult> GetSections([FromQuery] Guid boardId)
    {
        if (boardId == Guid.Empty)
        {
            return BadRequest(new { error = "BoardId is required" });
        }

        var userId = GetCurrentUserId();
        var sections = await _sectionService.GetSectionsByBoardIdAsync(boardId, userId);
        return Ok(sections);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSection([FromBody] CreateSectionRequest request)
    {
        if (request.BoardId == Guid.Empty)
        {
            return BadRequest(new { error = "BoardId is required" });
        }

        // Verify user has access to the board
        var userId = GetCurrentUserId();
        var hasAccess = await _sectionService.GetSectionsByBoardIdAsync(request.BoardId, userId);
        if (hasAccess == null || !hasAccess.Any())
        {
            return Unauthorized(new { error = "You don't have access to this board" });
        }

        var section = new Section
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BoardId = request.BoardId,
            Position = request.Position,
            IsDefault = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdSection = await _sectionService.CreateSectionAsync(section);
        return Ok(createdSection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSection(Guid id, [FromBody] UpdateSectionRequest request)
    {
        var userId = GetCurrentUserId();
        var section = await _sectionService.GetSectionByIdAsync(id, userId);
        
        if (section == null)
            return NotFound(new { error = $"Section {id} not found or you don't have access" });

        if (section.IsDefault)
            return BadRequest(new { error = "Cannot rename the default section" });

        var updated = await _sectionService.UpdateSectionNameAsync(id, request.Name, userId);
        if (!updated)
            return BadRequest(new { error = "Update failed" });

        return Ok(new { message = "Section updated successfully", name = request.Name });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSection(Guid id)
    {
        var userId = GetCurrentUserId();
        var section = await _sectionService.GetSectionByIdAsync(id, userId);
        
        if (section == null)
            return NotFound(new { error = $"Section {id} not found or you don't have access" });

        if (section.IsDefault)
            return BadRequest(new { error = "Cannot delete the default section" });

        var deleted = await _sectionService.DeleteSectionAsync(id, userId);
        if (!deleted)
            return BadRequest(new { error = "Delete failed" });

        return NoContent();
    }
}