using System.Text.Json;

namespace HomeworkGB9
{
    public class Message
    {
        public string FromName { get; set; } = "";
        public string ToName { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime Time { get; set; }
        public int? Id { get; set; } 
        public Message()
        {
            Time = DateTime.Now;
        }
        public Message(string name) : this()
        {
            FromName = name;
        }
        public Message(string name, string text) : this(name)
        {
            Text = text;
        }
        public Message(string fromName, string toName, string text) : this(fromName, text)
        {
            ToName = toName;
        }
        public string GetJson() => JsonSerializer.Serialize(this);
        public static Message? GetMessage(string json) => JsonSerializer.Deserialize<Message>(json);
        public static Message ConvertFromDatabase(Model.Message messageEntity)
        {
            var convertedMessage = new Message()
            {
                FromName = messageEntity.Sender?.Name ?? "",
                ToName = messageEntity.Recipient?.Name ?? "",
                Id = messageEntity.Id,
                Text = messageEntity.Text ?? "",
                Time = messageEntity.CreationTime
            };
            return convertedMessage;
        }
        public override string ToString() => $"От {FromName}, {Time}: {Text}";
    }
}
