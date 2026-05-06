namespace task_management_system_aca.Entities;

public class Board
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int OwnerId { get; set; }  // Make sure this is OwnerId, not OwnerId
    public DateTime CreatedAt { get; set; }
}