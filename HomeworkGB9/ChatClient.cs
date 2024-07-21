using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatClient
    {
        public static void SendMessage(int port, string nick)
        {
            IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), port);
            UdpClient udp = new();
            while (true)
            {
                Console.WriteLine("Enter your message:");
                string? text = Console.ReadLine();
                if (string.IsNullOrEmpty(text)) continue;
                else if (string.Equals(text, "exit", StringComparison.InvariantCultureIgnoreCase)) break;
                try
                {
                    Message msg = new(nick, text);
                    string json = msg.GetJson();
                    udp.Send(Encoding.UTF8.GetBytes(json), endPoint);
                    byte[] buffer = udp.Receive(ref endPoint);
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
