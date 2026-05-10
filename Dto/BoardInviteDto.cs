namespace task_management_system_aca.Dto;

public class BoardInviteDto
{
    public int Id { get; set; }
    public int BoardId { get; set; }
    public string BoardTitle { get; set; } = string.Empty;
    public string InvitedByUsername { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";  
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}