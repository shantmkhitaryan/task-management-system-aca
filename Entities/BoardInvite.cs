using Microsoft.AspNetCore.Mvc;

namespace task_management_system_aca.Entities
{
    public class BoardInvite
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public int Sender {  get; set; }
        public int Receiver { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
