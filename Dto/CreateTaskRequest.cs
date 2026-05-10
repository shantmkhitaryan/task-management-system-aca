using System.ComponentModel.DataAnnotations;

namespace task_management_system_aca.Dto;

public class CreateTaskRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public Guid? AssigneeId { get; set; }  
    
    public DateTime? DueDate { get; set; }
    
    public string Priority { get; set; } = "Medium";
}