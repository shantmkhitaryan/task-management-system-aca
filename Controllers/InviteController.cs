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
        public async Task<IActionResult> GetInvites([FromQuery] Guid boardId)
        {
            if (boardId == Guid.Empty)
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
                var invite = new BoardInvite
                {
                    Id = Guid.NewGuid(),  // Generate new Guid
                    BoardId = request.BoardId,
                    InvitedBy = request.Sender,
                    InvitedUserId = request.Receiver,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                await _inviteService.CreateInvitationAsync(invite);

                return Ok(new { id = invite.Id, message = "Invitation sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}