namespace task_management_system_aca.Dto;

public class CreateInviteRequest
{
    public Guid BoardId { get; set; }      
    public Guid Sender { get; set; }        
    public Guid Receiver { get; set; }      
}