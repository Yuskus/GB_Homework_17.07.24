using System.Net.Sockets;
using System.Net;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatServer
    {
        public static void AcceptMessage(int port)
        {
            IPEndPoint endPoint = new(IPAddress.Any, 0);
            UdpClient udp = new(port);
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
                        string backJson = message.GetJson();
                        byte[] backBuffer = Encoding.UTF8.GetBytes(backJson);
                        udp.Send(backBuffer, endPoint);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Console.WriteLine("Error.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
