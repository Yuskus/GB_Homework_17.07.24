namespace HomeworkGB9.Model
{
    public class Message
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsRecieved { get; set; }
        public int? SenderId { get; set; }
        public virtual User? Sender { get; set; }
        public int? RecipientId { get; set; }
        public virtual User? Recipient { get; set; }
    }
}
