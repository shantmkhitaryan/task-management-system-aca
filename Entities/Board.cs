namespace task_management_system_aca.Entities;

public class Board
{
    public Guid Id { get; set; } = Guid.NewGuid();  
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }  
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    public User Owner { get; set; } = null!;
    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}