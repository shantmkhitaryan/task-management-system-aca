namespace task_management_system_aca.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
   
    public ICollection<Board> Boards { get; set; } = new List<Board>();
    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}