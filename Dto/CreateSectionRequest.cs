namespace task_management_system_aca.Dto;

public class CreateSectionRequest
{
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
}