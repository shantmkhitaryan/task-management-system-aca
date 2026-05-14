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

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingInvites([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest(new { error = "UserId is required" });
            }

            var invites = await _inviteService.GetInvitesByUserAsync(userId);
            return Ok(invites);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvite([FromBody] CreateInviteRequest request)
        {
            try
            {
                var invite = new BoardInvite
                {
                    Id = Guid.NewGuid(),
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

        
        [HttpPatch("{inviteId}")]
        public async Task<IActionResult> UpdateInviteStatus(Guid inviteId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { error = "Status is required. Use 'Accepted' or 'Rejected'" });
            }

            if (status != "Accepted" && status != "Rejected")
            {
                return BadRequest(new { error = "Status must be 'Accepted' or 'Rejected'" });
            }

            var invite = await _inviteService.GetInvitationByIdAsync(inviteId);
            if (invite == null)
            {
                return NotFound(new { error = $"Invitation {inviteId} not found" });
            }

            if (invite.Status != "Pending")
            {
                return BadRequest(new { error = $"Invitation is already {invite.Status}" });
            }

            if (invite.ExpiresAt < DateTime.UtcNow)
            {
                await _inviteService.UpdateInvitationStatusAsync(inviteId, "Expired");
                return BadRequest(new { error = "Invitation has expired" });
            }

            var updated = await _inviteService.UpdateInvitationStatusAsync(inviteId, status);
            if (!updated)
            {
                return BadRequest(new { error = "Failed to update invitation status" });
            }

            
            if (status == "Accepted")
            {
                await _inviteService.AddUserToBoardAsync(invite.BoardId, invite.InvitedUserId);
            }

            return Ok(new { message = $"Invitation {status.ToLower()} successfully", inviteId, status });
        }

        [HttpDelete("{inviteId}")]
        public async Task<IActionResult> DeleteInvite(Guid inviteId)
        {
            var deleted = await _inviteService.DeleteInvitationAsync(inviteId);
            if (!deleted)
                return NotFound(new { error = $"Invite {inviteId} not found" });

            return Ok(new { message = $"Invite {inviteId} deleted successfully" });
        }
    }
}