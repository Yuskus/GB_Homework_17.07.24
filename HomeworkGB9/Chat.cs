using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9
{
    internal class Chat
    {
        public const int PORT = 12345;
        public static void Server()
        {
            IPEndPoint endPoint = new(IPAddress.Any, 0);
            UdpClient udp = new(PORT);
            Console.WriteLine("Connecting...");
            while (true)
            {
                try
                {
                    byte[] buffer = udp.Receive(ref endPoint);
                    string json = Encoding.UTF8.GetString(buffer);
                    Message? msg = Message.GetMessage(json);
                    if (msg != null)
                    {
                        Console.WriteLine(msg);
                        Message message = new("Server", "Message sent.");
                        byte[] backBuffer = Encoding.UTF8.GetBytes(message.GetJson());
                        udp.Send(backBuffer, endPoint);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Console.WriteLine("Error.");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public static void Client(string nick)
        {
            IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), PORT);
            UdpClient udp = new();
            while (true)
            {
                Console.WriteLine("Enter your message:");
                string? text = Console.ReadLine();
                if (string.IsNullOrEmpty(text)) continue;
                Message msg = new(nick, text);
                string json = msg.GetJson();
                udp.Send(Encoding.UTF8.GetBytes(json), endPoint);
                byte[] buffer = udp.Receive(ref endPoint);
                string backJson = Encoding.UTF8.GetString(buffer);
                Message? message = Message.GetMessage(backJson);
                Console.WriteLine(message);
            }
        }
    }
}
