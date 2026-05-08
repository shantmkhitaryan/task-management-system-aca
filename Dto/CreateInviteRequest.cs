using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;

namespace task_management_system_aca.Dto
{
    public class CreateInviteRequest
    {
        [Required]
        public int BoardId { get; set; }
        [Required]
        public int Sender { get; set; }
        [Required]
        public int Receiver {  get; set; }
        [Required]
        public string Status { get; set; } = "Pending";
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
