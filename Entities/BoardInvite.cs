namespace task_management_system_aca.Entities;

public class BoardInvite
{
    public Guid Id { get; set; } = Guid.NewGuid();  
    public Guid BoardId { get; set; }  
    public Guid InvitedUserId { get; set; }  
    public Guid InvitedBy { get; set; }  
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);
    
    
    public Board Board { get; set; } = null!;
    public User InvitedUser { get; set; } = null!;
    public User Inviter { get; set; } = null!;
}