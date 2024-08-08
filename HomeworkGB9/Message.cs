using System.Text.Json;

namespace HomeworkGB9
{
    public enum Command
    {
        Message,
        Register,
        Delete,
        Confirm
    }
    public class Message
    {
        public int? Id { get; set; }
        public string FromName { get; set; } = "";
        public string ToName { get; set; } = "";
        public string Text { get; set; } = "";
        public Command Command { get; set; } = Command.Message;
        public DateTime Time { get; set; }
        
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
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Message? ToMessage(string json)
        {
            return JsonSerializer.Deserialize<Message>(json);
        }
        public static Message ConvertFromDatabase(Model.Message messageEntity)
        {
            return new Message()
            {
                FromName = messageEntity.Sender?.Name ?? "",
                ToName = messageEntity.Recipient?.Name ?? "",
                Id = messageEntity.Id,
                Text = messageEntity.Text ?? "",
                Time = messageEntity.CreationTime
            };
        }
        public static Model.Message ConvertToDatabase(Message message, Model.User? sender, Model.User? recipient)
        {
            return new Model.Message()
            {
                Text = message.Text,
                CreationTime = message.Time,
                IsReceived = false,
                SenderId = sender?.Id,
                RecipientId = recipient?.Id
            };
        }
        public override string ToString()
        {
            return Command == Command.Message ? $"От {FromName}, {Time}: {Text}" : $"От {FromName}, {Time}: [Command: {Command}] {Text}";
        }
    }
}
