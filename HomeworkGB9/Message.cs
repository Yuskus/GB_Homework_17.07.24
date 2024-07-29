using System.Text.Json;

namespace HomeworkGB9
{
    public class Message
    {
        public string FromName { get; set; } = "";
        public string ToName { get; set; } = "";
        public string Text { get; set; } = "";
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
        public string GetJson() => JsonSerializer.Serialize(this);
        public static Message? GetMessage(string json) => JsonSerializer.Deserialize<Message>(json);
        public override string ToString() => $"{FromName} ({Time}): {Text}";
    }
}
