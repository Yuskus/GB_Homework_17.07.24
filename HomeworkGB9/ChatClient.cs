using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatClient
    {
        public static async Task SendMessageAsync(int port, string nick)
        {
            IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), port);
            UdpClient udp = new();
            while (true)
            {
                Console.WriteLine("Enter your message:");
                string? text = Console.ReadLine();
                if (string.IsNullOrEmpty(text)) continue;
                if (string.Equals(text, "exit", StringComparison.InvariantCultureIgnoreCase)) break;
                try
                {
                    Message msg = new(nick, text);
                    string json = msg.GetJson();
                    await udp.SendAsync(Encoding.UTF8.GetBytes(json), endPoint);
                    UdpReceiveResult data = await udp.ReceiveAsync();
                    byte[] buffer = data.Buffer;
                    string backJson = Encoding.UTF8.GetString(buffer);
                    Message? message = Message.GetMessage(backJson);
                    Console.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
