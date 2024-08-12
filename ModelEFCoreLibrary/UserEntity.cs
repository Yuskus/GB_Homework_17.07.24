namespace ModelEFCoreLibrary
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public virtual ICollection<MessageEntity> FromMessages { get; set; }
        public virtual ICollection<MessageEntity> ToMessages { get; set; }
        public UserEntity()
        {
            FromMessages = [];
            ToMessages = [];
        }
    }
}
