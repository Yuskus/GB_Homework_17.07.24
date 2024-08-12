namespace ModelEFCoreLibrary
{
    public class MessageEntity
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsReceived { get; set; }
        public int? SenderId { get; set; }
        public virtual UserEntity? Sender { get; set; }
        public int? RecipientId { get; set; }
        public virtual UserEntity? Recipient { get; set; }
    }
}
