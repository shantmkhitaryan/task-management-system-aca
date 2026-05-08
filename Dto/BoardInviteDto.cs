namespace task_management_system_aca.Dto
{
    public class BoardInviteDto
    {
        public int Id { get; set; }
        public int InviteSender { get; set; }
        public int InviteReceiver { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
