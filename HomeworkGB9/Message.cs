using System.Text.Json;

namespace HomeworkGB9
{
    public class Message
    {
        public string? Name { get; set; }
        public string? Text { get; set; }
        public DateTime Time { get; set; }
        public Message() { }
        public Message(string name, string text)
        {
            Name = name;
            Text = text;
            Time = DateTime.Now;
        }

        public string GetJson() => JsonSerializer.Serialize(this);
        public static Message? GetMessage(string json) => JsonSerializer.Deserialize<Message>(json);
        public override string ToString() => $"From: {Name}\nSent: {Time}\nText: {Text}";
    }
}
