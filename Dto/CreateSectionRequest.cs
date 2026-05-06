using System.ComponentModel.DataAnnotations;

namespace task_management_system_aca.Dto;

public class CreateSectionRequest
{
    [Required]
    public int BoardId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int Position { get; set; }
}