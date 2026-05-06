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

    [HttpGet]
    public async Task<IActionResult> GetSections([FromQuery] int boardId)
    {
        if (boardId <= 0)
        {
            return BadRequest(new { error = "BoardId is required" });
        }
        
        var sections = await _sectionService.GetSectionsByBoardIdAsync(boardId);
        return Ok(sections);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSection([FromBody] CreateSectionRequest request)
    {
        try
        {
            var sectionId = new Random().Next(1, 1000000);
            
            var section = new Section
            {
                Id = sectionId,
                Name = request.Name,
                BoardId = request.BoardId,
                Position = request.Position,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow
            };

            await _sectionService.CreateSectionAsync(section);
            
            return Ok(new { id = sectionId, message = "Section created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}