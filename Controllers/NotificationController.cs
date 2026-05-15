using Microsoft.AspNetCore.Mvc;
using task_management_system_aca.Services;

namespace task_management_system_aca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(Guid userId)
    {
        var result = await _notificationService.GetNotificationsAsync(userId);
        return Ok(result);
    }

    [HttpPatch("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, [FromQuery] Guid userId)
    {
        var success = await _notificationService.MarkAsReadAsync(notificationId, userId);
        if (!success) return NotFound("Notification not found.");
        return Ok();
    }

    [HttpPatch("{userId}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(Guid userId)
    {
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok();
    }
}