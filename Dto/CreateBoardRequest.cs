using System.ComponentModel.DataAnnotations;

namespace task_management_system_aca.Dto;

public class CreateBoardRequest
{
    [Required]
    public Guid UserId { get; set; }  
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public string Sku { get; set; } = string.Empty;
}