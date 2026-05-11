using System.ComponentModel.DataAnnotations.Schema;

namespace task_management_system_aca.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid BoardId { get; set; }
    public Guid SectionId { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public bool IsArchived { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    
    [NotMapped]
    public string TaskReference 
    { 
        get 
        {
            var shortId = Id.ToString().Split('-').Last().Substring(0, 4).ToUpper();
            return $"{Board?.Sku ?? "???"}-{shortId}";
        }
    }
    
    
    [NotMapped]
    public string DueDateState
    {
        get
        {
            if (!DueDate.HasValue)
                return "None";

            var today = DateTime.UtcNow.Date;
            var dueDate = DueDate.Value.Date;

            if (today > dueDate)
                return "Overdue";
            
            if (today == dueDate.AddDays(-1))
                return "Attention";
            
            return "Normal";
        }
    }
    
    
    [ForeignKey("BoardId")]
    public Board Board { get; set; } = null!;
    
    [ForeignKey("SectionId")]
    public Section Section { get; set; } = null!;
    
    [ForeignKey("AssigneeId")]
    public User? Assignee { get; set; }
    
    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;
}