namespace task_management_system_aca.Entities;

public class Section
{
    public Guid Id { get; set; } = Guid.NewGuid();  
    public string Name { get; set; } = string.Empty;
    public Guid BoardId { get; set; } 
    public int Position { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    public Board Board { get; set; } = null!;
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}