namespace task_management_system_aca.Dto;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string TaskReference { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid BoardId { get; set; }
    public Guid SectionId { get; set; }
    public Guid? AssigneeId { get; set; }
    public DateTime? DueDate { get; set; }
    public string DueDateState { get; set; } = "None";
    public string Priority { get; set; } = "Medium";
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}