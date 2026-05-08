using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Dto;
using task_management_system_aca.Entities;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InviteController : ControllerBase
    {
        private readonly InviteService _inviteService;

        public InviteController(InviteService inviteService)
        {
            _inviteService = inviteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvites([FromQuery] int boardId)
        {
            if (boardId <= 0)
            {
                return BadRequest(new { error = "BoardId is required" });
            }

            var invites = await _inviteService.GetBoardInvitesByBoardIdAsync(boardId);
            return Ok(invites);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest request)
        {
            try
            {
                var inviteId = new Random().Next(1, 1000000);

                var invite = new BoardInvite
                {
                    Id = inviteId,
                    BoardId = request.BoardId,
                    Sender = request.Sender,
                    Receiver = request.Receiver,
                    Status = request.Status,
                    CreatedAt = DateTime.UtcNow
                };

                await _inviteService.CreateInvitationAsync(invite);

                return Ok(new { id = inviteId, message = "Invitation sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
